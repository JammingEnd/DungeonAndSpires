using MegaCrit.Sts2.Core.Entities.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class WishPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}