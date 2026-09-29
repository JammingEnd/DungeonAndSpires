using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Statusses;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

[Pool(typeof(DASSpellCardPool))]
public abstract class SpellCard(int Level, int cost, CardType type, CardRarity rarity, TargetType target) : CoreCard(cost, type, rarity, target)
{
    public int Level { get; } = Level;

    // Some card (primarily cards that apply powers) should not consume a spellslot
    // the power 'Shield' Consumes a spellslot, the card who applies should not
    internal bool _shouldConsumeSpellslot = true;

    /// <summary>
    /// Whether playing this spell consumes a spellslot (and gets the -1 energy discount when one
    /// is available). False for spells that shouldn't interact with spellslots (cards like Shield, since only the powers itself consumed spellslots).
    /// </summary>
    public virtual bool UsesSpellSlot => true;

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
        // pre-play energy for Ritual's spellslot decision.
        _shouldConsumeSpellslot = ShouldConsumeSpellslot();
        if (Keywords.Contains(CoreKeywords.Scrolled))
        {
            _shouldConsumeSpellslot = false;
            modifiedCost = 0;
            return true;
        }
        if (Level > 0 && UsesSpellSlot && Owner.PlayerCombatState?.HasAvailableSlotForLevel(Level) == true)
        {
            modifiedCost = originalCost - 1;
            return true;
        }
        return false;
    }

    // Consume the spellslot when a leveled spell is cast.
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);
        int Metamagic = cardPlay.Player.PlayerCombatState.GetMetamagic();
        if (cardPlay.Card == this && Level > 0 && UsesSpellSlot && _shouldConsumeSpellslot && Owner.PlayerCombatState?.HasAvailableSlotForLevel(Level) == true)
        {
            await SpellslotsCmd.ConsumeSpellSlotForLevel(choiceContext, Owner, Level);
        }
        else if(cardPlay.Card == this && Level > 0 && UsesSpellSlot && _shouldConsumeSpellslot &&  Owner.PlayerCombatState?.HasAvailableSlotForLevel(Level) == false && Metamagic > 0)
        {
            await MetamagicCmd.Remove(choiceContext, cardPlay.Player, 1);
        }
        else if (cardPlay.Card == this && Level > 0 && _shouldConsumeSpellslot)
        {
                var card = (ArcaneExhaustion)CombatState.CreateCard(ModelDb.Card<ArcaneExhaustion>(), cardPlay.Player);
                SpellCard thisCard = cardPlay.Card as SpellCard;
                card.SetLevel(thisCard.Level);
                var cardview = await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, cardPlay.Player);
                CardCmd.PreviewCardPileAdd(cardview, 0.5f);
        }
    }

    // Ritual spells don't consume a spellslot when your energy is above half its maximum. when 3 is max, you need more than 2
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
}