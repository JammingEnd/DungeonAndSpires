using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class LesserRestorationPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([
            new IntVar("Strength", 0),
            new IntVar("Dexterity", 0)
        ]);

    public void SetGains(int strength, int dexterity)
    {
        DynamicVars["Strength"].BaseValue = strength;
        DynamicVars["Dexterity"].BaseValue = dexterity;
    }

    public override bool TryModifyPowerAmountReceived(PowerModel canonicalPower, Creature target, decimal amount, Creature? _, out decimal modifiedAmount)
    {
        modifiedAmount = amount;
        if (target != Owner || Amount <= 0 || amount <= 0)
        {
            return false;
        }
        if (canonicalPower is not (WeakPower or FrailPower))
        {
            return false;
        }
        modifiedAmount = 0m;
        return true;
    }

    public override async Task AfterModifyingPowerAmountReceived(PowerModel power)
    {
        var context = new ThrowingPlayerChoiceContext();
        await PowerCmd.Apply<StrengthPower>(context, Owner, DynamicVars["Strength"].IntValue, Owner, null);
        await PowerCmd.Apply<DexterityPower>(context, Owner, DynamicVars["Dexterity"].IntValue, Owner, null);
        await PowerCmd.Remove(this);
    }
}