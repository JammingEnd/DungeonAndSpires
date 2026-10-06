using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Sorcerer;

[Pool(typeof(SorcererRelicPool))]
public class LunarSorcererRelic: SubclassRelic, IOnMetamagicChanged
{
    public override RelicRarity Rarity => RelicRarity.None;
    public override async Task InitialBonus()
    {
        UnlockedEnergyGain = true;
    }
    
    private bool unlockedEnergyGain = false;
    private bool usedEnergyFeature = false;
    [SavedProperty]
    public bool UnlockedEnergyGain
    {
        get => unlockedEnergyGain;
        set => unlockedEnergyGain = value;
    }

    [SavedProperty]
    public bool UsedEnergyFeature
    {
        get => usedEnergyFeature;
        set => usedEnergyFeature = value;
    }
    public async Task OnMetamagicChanged(PlayerChoiceContext choiceContext, Player player, int current, int currentAfterChange)
    {
        if(!unlockedEnergyGain || usedEnergyFeature)
            return;
        if (currentAfterChange <= 0)
        {
            await PlayerCmd.GainEnergy(2, player);
        }
    }

    public override async Task BeforeCombatStart()
    {
        UsedEnergyFeature = false;
    }

    public override async Task Act2Bonus()
    {
        UnlockedHeal = true;
    }
    
    private bool unlockedHeal = false;
    [SavedProperty]
    public bool UnlockedHeal
    {
        get => unlockedHeal;
        set => unlockedHeal = value;
    }

    private bool _wasSpellAttack;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // for act2 healing
        if (cardPlay.Card is SpellCard && cardPlay.Card.Type == CardType.Attack)
        {
            _wasSpellAttack = true;
        }
        else
        {
            _wasSpellAttack = false;
        }
        
        if (cardPlay.Player != Owner)
            return;
        // for act3 draw
        if (UnlockedDraw && cardPlay.Card is SpellCard spell)
        {
            if (spell.Keywords.Any(x =>
                    x == CoreKeywords.Quickened || x == CoreKeywords.Scrolled || x == CoreKeywords.Heightened))
            {
                await CardPileCmd.Draw(choiceContext, 1, Owner);
            }
        }
    }

    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, bool wasRemovalPrevented, float deathAnimLength)
    {
        if (UnlockedHeal && !creature.IsPlayer)
        {
            if (_wasSpellAttack && Owner.Creature.CombatState.HittableEnemies.Count <= 0)
            {
                await CreatureCmd.Heal(Owner.Creature, 5m);
            }
        }
    }

    public override async Task Act3Bonus()
    {
        UnlockedDraw = true;
    }
    private bool unlockedDraw = false;
    [SavedProperty]
    public bool UnlockedDraw
    {
        get => unlockedDraw;
        set => unlockedDraw = value;
    }
}