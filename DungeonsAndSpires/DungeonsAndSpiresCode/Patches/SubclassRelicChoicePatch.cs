using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(Player), "PopulateStartingRelics")]
internal class SubclassRelicChoicePatch
{
    [HarmonyPostfix]
    private static void Postfix(Player __instance)
    {
        if (__instance.Character is not SorcererCharacter)
        {
            return;
        }

        var selected = SubclassRelicPicker.SelectedSubclassRelic;
        if (selected == null)
        {
            return;
        }

        async Task SwapRelic()
        {
            // The Sorcerer starts with a default subclass relic; replace it with the picked one.
            var granted = __instance.Relics.OfType<SubclassRelic>().FirstOrDefault();
            if (granted != null && granted.GetType() != selected.GetType())
            {
                await RelicCmd.Remove(granted);
            }
            if (!__instance.Relics.Any(r => r.GetType() == selected.GetType()))
            {
                await RelicCmd.Obtain(selected.ToMutable(), __instance);
            }
        }

        TaskHelper.RunSafely(SwapRelic());
    }
}