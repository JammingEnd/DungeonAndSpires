using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class PrayerOfHealingPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([
            new IntVar("Strength", 0),
            new IntVar("Dexterity", 0)
        ]);

    public void SetAmounts(decimal strength, decimal dexterity)
    {
        DynamicVars["Strength"].BaseValue = strength;
        DynamicVars["Dexterity"].BaseValue = dexterity;
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player)
        {
            return;
        }
        if (card.Type != CardType.Status && card.Type != CardType.Curse)
        {
            return;
        }

        await PowerCmd.Apply<PrayerOfHealingStrengthPower>(choiceContext, Owner, DynamicVars["Strength"].IntValue, Owner, null);
        await PowerCmd.Apply<PrayerOfHealingDexterityPower>(choiceContext, Owner, DynamicVars["Dexterity"].IntValue, Owner, null);

        await PowerCmd.Remove(this);
    }
}