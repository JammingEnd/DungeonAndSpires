using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class PrayerOfHealingStrengthPower() : CustomTemporaryPowerModelWrapper<PrayerOfHealing, StrengthPower>
{
    public override PowerType Type => PowerType.Buff;

    protected override bool InvertInternalPowerAmount => false;
}

public class PrayerOfHealingDexterityPower() : CustomTemporaryPowerModelWrapper<PrayerOfHealing, DexterityPower>
{
    public override PowerType Type => PowerType.Buff;

    protected override bool InvertInternalPowerAmount => false;
}