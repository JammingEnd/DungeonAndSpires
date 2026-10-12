using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public static class DipField
{
    public static readonly SpireField<PlayerCombatState, DipState> State = new(() => null);
}