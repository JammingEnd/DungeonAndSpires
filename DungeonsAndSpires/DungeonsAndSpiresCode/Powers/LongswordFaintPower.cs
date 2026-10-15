using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class LongswordFaintPower() : CustomTemporaryPowerModelWrapper<LongswordFaint, StrengthPower>
{
    public override PowerType Type => PowerType.Debuff;

    protected override bool InvertInternalPowerAmount => true;
}
