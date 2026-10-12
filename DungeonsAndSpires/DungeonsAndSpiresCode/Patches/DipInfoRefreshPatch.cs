using DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(NCard), "Reload")]
internal static class DipInfoRefreshPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        try
        {
            var control = AddDipInfoToCards.AddedNode[__instance];
            if (control == null || !GodotObject.IsInstanceValid(control))
            {
                return;
            }

            control.Refresh();
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"DipInfoRefresh failed: {e}");
        }
    }
}