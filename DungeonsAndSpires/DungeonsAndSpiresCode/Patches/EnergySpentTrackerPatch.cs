using DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), nameof(PlayerCombatState.LoseEnergy))]
internal static class EnergySpentTrackerPatch
{
    [HarmonyPostfix]
    private static void Postfix(PlayerCombatState __instance, decimal amount)
    {
        if (amount <= 0m)
        {
            return;
        }

        var state = __instance.GetWeaponPlay();
        if (state != null)
        {
            state.EnergySpent += (int)amount;
        }
    }
}
