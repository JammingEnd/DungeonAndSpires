using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ModifyMemoryPower() : CustomTemporaryPowerModelWrapper<ModifyMemory, StrengthPower>
{
    public override PowerType Type => PowerType.Debuff;

    protected override bool InvertInternalPowerAmount => true;
}