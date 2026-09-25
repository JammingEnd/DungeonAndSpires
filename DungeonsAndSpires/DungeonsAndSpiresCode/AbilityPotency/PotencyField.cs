using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

public static class PotencyField
{
    public static readonly SpireField<PlayerCombatState, PotencyState> State = new(() => null);
}