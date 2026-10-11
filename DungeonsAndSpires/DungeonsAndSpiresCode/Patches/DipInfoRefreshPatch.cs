using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

/// <summary>
/// Refreshes a card view's DipCardInfo overlay whenever the view reloads (draw, redraw, turn-end
/// reshuffle, etc.), so it stays tied to whatever card the view currently displays.
/// </summary>
[HarmonyPatch(typeof(NCard), "Reload")]
internal static class DipInfoRefreshPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        if (__instance.Model == null)
        {
            return;
        }

        var control = __instance.GetNodeOrNull<Control>("%CardContainer")?.GetNodeOrNull<Control>("DipCardInfo");
        if (control == null)
        {
            return;
        }

        DipCmd.SetUI(__instance.Model, true);
    }
}