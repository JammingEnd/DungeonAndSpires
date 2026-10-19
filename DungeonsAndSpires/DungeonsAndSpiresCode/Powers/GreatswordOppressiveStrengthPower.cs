using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// Temporary Strength added by Greatsword Oppressive; dropped when the two-handed chain breaks.
public class GreatswordOppressiveStrengthPower() : BaseLib.Abstracts.CustomTemporaryPowerModelWrapper<GreatswordOppressive, StrengthPower>
{
    public override PowerType Type => PowerType.Buff;

    protected override bool InvertInternalPowerAmount => false;
}
