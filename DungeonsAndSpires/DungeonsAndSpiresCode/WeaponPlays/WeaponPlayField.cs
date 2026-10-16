using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;

public static class WeaponPlayField
{
    public static readonly SpireField<PlayerCombatState, WeaponPlayState> State = new(() => null);
}
