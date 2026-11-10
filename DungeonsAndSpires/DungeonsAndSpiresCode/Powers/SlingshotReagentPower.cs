using MegaCrit.Sts2.Core.Entities.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// Consumption happens in WeaponDipsHandler: powers run before combat singletons, so removing it here would eat the buff first.
public class SlingshotReagentPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
}
