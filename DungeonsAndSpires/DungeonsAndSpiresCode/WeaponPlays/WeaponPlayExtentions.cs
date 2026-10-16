using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;

public static class WeaponPlayExtentions
{
    public static WeaponPlayState? GetWeaponPlay(this PlayerCombatState state)
    {
        return WeaponPlayField.State[state];
    }
}
