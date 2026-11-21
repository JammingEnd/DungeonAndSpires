using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class SlingshotBullseyeDexterityPower() : CustomTemporaryPowerModelWrapper<SlingshotBullseye, DexterityPower>
{
    public override PowerType Type => PowerType.Buff;

    protected override bool InvertInternalPowerAmount => false;
}
