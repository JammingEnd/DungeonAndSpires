using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

// heightened is a scaling factor now
public static class HeightenedField
{
    public static readonly SpireField<CardModel, int> Level = new(() => 0);
}
