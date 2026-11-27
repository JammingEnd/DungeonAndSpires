using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Statusses;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

[Pool(typeof(DASSpellCardPool))]
public abstract class SpellCard(int Level, int cost, CardType type, CardRarity rarity, TargetType target) : CoreCard(cost, type, rarity, target)
{
    public int Level { get; } = Level;

    /// <summary>If true, the card will use and consume vigor and use Strength</summary>
    public virtual bool UsesPoweredAttack => false;

    /// <summary> If false, the cardplay will not consume a spellslot (some powers like Shield of Faith) </summary>
    internal bool _shouldConsumeSpellslot = true;
    
    private bool _warlockAboveSlot;

    private bool IsWarlock => Owner.Character.CardPool is WarlockCardPool;

    private int WarlockSlotLevel => Owner.Creature.GetPower<EmpowerPatronPower>()?.Amount ?? 1;

    /// <summary>
    /// Whether playing this spell consumes a spellslot (and gets the -1 energy discount when one
    /// is available). False for spells that shouldn't interact with spellslots (cards like Shield, since only the powers itself consumed spellslots).
    /// </summary>
    public virtual bool UsesSpellSlot => true;

    /// <summary>
    /// Whether casting this spell without a slot/metamagic should leave an Arcane Exhaustion behind.
    /// Override on a card, or veto it from a relic/power via <see cref="IShouldCreateExhaustion"/>.
    /// </summary>
    public virtual bool ShouldCreateExhaustion(PlayerChoiceContext choiceContext, Player player) => true;
    
    
    public virtual string AttackAnimName => "cast_2";

    // Spells with a spell level of 0 are cantrips.
    protected override HashSet<CardTag> CanonicalTags => Level == 0 ? [DASCoreCardtags.Spell, DASCoreCardtags.Cantrip] : [DASCoreCardtags.Spell];

    // Spells cost 1 less energy while a spellslot of their level is available. Scrolled spells are free.
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card != this)
        {
            return false;
        }
        // So like.... apparently TryModify is literally BeforeCardPlay
        _shouldConsumeSpellslot = ShouldConsumeSpellslot();
        if (Keywords.Contains(CoreKeywords.Scrolled))
        {
            _shouldConsumeSpellslot = false;
            modifiedCost = 0;
            return true;
        }
        if (Level > 0 && UsesSpellSlot)
        {
            bool hasSlot = IsWarlock ? Owner.PlayerCombatState?.HasAvailableSlotForLevel(1) == true : Owner.PlayerCombatState?.HasAvailableSlotForLevel(Level) == true;

            if (IsWarlock)
            {
                ComputeWarlockCast(hasSlot);
            }

            if (hasSlot)
            {
                modifiedCost = originalCost - 1;
                return true;
            }
        }
        return false;
    }

    private void ComputeWarlockCast(bool hasSlot)
    {
        int slotLevel = WarlockSlotLevel;
        _warlockAboveSlot = !hasSlot || Level > slotLevel;
        int heightened = _warlockAboveSlot ? 0 : (slotLevel - Level) / 2;
        HeightenedField.Level[this] = heightened;
        if (heightened > 0 && !Keywords.Contains(CoreKeywords.Heightened))
        {
            this.AddKeyword(CoreKeywords.Heightened);
        }
    }

    // Consume the spellslot when a leveled spell is cast.
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);

        if (cardPlay.Card == this && Level > 0 && UsesSpellSlot && _shouldConsumeSpellslot)
        {
            if (IsWarlock)
            {
                await HandleWarlockSpellCast(choiceContext, cardPlay);
            }
            else
            {
                await HandleStandardSpellCast(choiceContext, cardPlay);
            }
        }

        if (cardPlay.Card == this && Keywords.Contains(CoreKeywords.Heightened))
        {
            this.RemoveKeyword(CoreKeywords.Heightened);
        }
        HeightenedField.Level[this] = 0;
        _warlockAboveSlot = false;
        // Quickened cards also deal their effects to a random enemy (or ally).
        if (cardPlay.Card == this && Keywords.Contains(CoreKeywords.Quickened))
        {
            this.RemoveKeyword(CoreKeywords.Quickened);

            if (cardPlay.Card.TargetType == TargetType.Self)
            {
                var otherPlayers = CombatState.Players.Where(p => p != Owner).ToArray();
                if (otherPlayers.Length > 0)
                {
                    var ally = otherPlayers[Owner.RunState.Rng.CombatTargets.NextInt(otherPlayers.Length)];
                    await CardCmd.AutoPlay(choiceContext, this, ally.Creature, skipXCapture: true);
                }
            }
            else
            {
                var hittable = CombatState.HittableEnemies.ToList();
                if (hittable.Count == 0)
                {
                    return;
                }

                // quickened targets different enemy when more are present on the map
                var candidates = hittable.Count > 1 ? hittable.Where(e => e != cardPlay.Target).ToArray() : hittable.ToArray();
                if (candidates.Length == 0)
                {
                    candidates = hittable.ToArray();
                }

                var enemy = Owner.RunState.Rng.CombatTargets.NextItem(candidates);
                if (enemy != null)
                {
                    await CardCmd.AutoPlay(choiceContext, this, enemy, AutoPlayType.Default, true);
                }
            }
        } 
        
        if (cardPlay.Card == this && Keywords.Contains(CoreKeywords.Scrolled))
        {
            await CardCmd.Exhaust(choiceContext, this);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
            return;
        
        if (Keywords.Contains(CoreKeywords.Heightened))
        {
            this.RemoveKeyword(CoreKeywords.Heightened);
        }
        if (Keywords.Contains(CoreKeywords.Quickened))
        {
            this.RemoveKeyword(CoreKeywords.Quickened);
        }
        HeightenedField.Level[this] = 0;
        _warlockAboveSlot = false;
    }


    // Ritual spells dont consume a spellslot when your energy is above half its maximum. when 3 is max, you need more than 2
    private bool ShouldConsumeSpellslot()
    {
        if (Keywords.Contains(CoreKeywords.Ritual))
        {
            int energy = Owner.PlayerCombatState?.Energy ?? 0;
            int maxEnergy = Owner.PlayerCombatState?.MaxEnergy ?? 0;
            if (energy > Math.Ceiling(maxEnergy / 2m))
            {
                return false;
            }
        }
        return true;
    }

    private async Task HandleStandardSpellCast(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int metamagic = cardPlay.Player.PlayerCombatState.GetMetamagic();
        if (Owner.PlayerCombatState?.HasAvailableSlotForLevel(Level) == true)
        {
            await SpellslotsCmd.ConsumeSpellSlotForLevel(choiceContext, Owner, Level);
        }
        else if (metamagic > 0)
        {
            await MetamagicCmd.Remove(choiceContext, cardPlay.Player, 1);
        }
        else if (SpellExhaustionHook.ShouldCreate(CombatState, this, choiceContext, cardPlay.Player))
        {
            await CreateArcaneExhaustion(choiceContext, cardPlay, Level);
        }
    }
    
    // warlock uses only one spellslot. read notes for a better explaination
    private async Task HandleWarlockSpellCast(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        if (Owner.PlayerCombatState?.HasAvailableSlotForLevel(1) == true)
        {
            await SpellslotsCmd.ConsumeSpellSlotForLevel(choiceContext, Owner, 1);
        }

        
        if (_warlockAboveSlot && SpellExhaustionHook.ShouldCreate(CombatState, this, choiceContext, cardPlay.Player))
        {
            await CreateArcaneExhaustion(choiceContext, cardPlay, Level);
        }
    }

    private async Task CreateArcaneExhaustion(PlayerChoiceContext choiceContext, CardPlay cardPlay, int level)
    {
        var card = (ArcaneExhaustion)CombatState.CreateCard(ModelDb.Card<ArcaneExhaustion>(), cardPlay.Player);
        card.SetLevel(level);
        var cardview = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, cardPlay.Player);
        CardCmd.PreviewCardPileAdd(cardview, 0.5f);
    }

    public override async Task OnEnqueuePlayVfx(Creature? target)
    {
        if (this.Type != CardType.Attack)
        {
            await CreatureCmd.TriggerAnim(Owner.Creature, "cast", 0);
        }
    }
}