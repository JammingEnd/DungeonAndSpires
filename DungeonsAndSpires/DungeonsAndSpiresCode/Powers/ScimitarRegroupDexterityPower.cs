using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ScimitarRegroupDexterityPower() : CustomTemporaryPowerModelWrapper<ScimitarRegroup, DexterityPower>
{
    public override PowerType Type => PowerType.Buff;

    protected override bool InvertInternalPowerAmount => false;
}
