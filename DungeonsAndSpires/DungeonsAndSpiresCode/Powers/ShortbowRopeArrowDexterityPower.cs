using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortbowRopeArrowDexterityPower() : CustomTemporaryPowerModelWrapper<ShortbowRopeArrow, DexterityPower>
{
    public override PowerType Type => PowerType.Debuff;

    protected override bool InvertInternalPowerAmount => true;
}