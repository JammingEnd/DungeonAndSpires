using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public abstract class SpiritPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    protected int _level;
    
    public void SetLevel(int level) => _level = level;
}