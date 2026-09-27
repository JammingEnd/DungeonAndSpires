using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class Message() : SpellCard(0, 1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar(2),
        new IntVar("MaxLevel", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawn = await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);

        if (drawn.Any(c => c.EnergyCost.GetResolved() == 0))
        {
            int maxLevel = DynamicVars["MaxLevel"].IntValue;
            for (int level = maxLevel; level >= 1; level--)
            {
                var slot = Owner.PlayerCombatState?.GetSpellslotForLevel(level);
                if (slot != null && slot.GetCurrent() < slot.GetMax())
                {
                    await SpellslotsCmd.AddSpellSlotForLevel(choiceContext, Owner, level);
                    return;
                }
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
        DynamicVars["MaxLevel"].UpgradeValueBy(1);
    }
}