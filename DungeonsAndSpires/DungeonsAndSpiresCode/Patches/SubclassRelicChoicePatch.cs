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
        if (__instance.Character is not (SorcererCharacter or FighterCharacter or RogueCharacter))
        {
            return;
        }

        var character = __instance.Character;
        var selected = SubclassRelicPicker.SelectedSubclassRelic;
        
        bool belongsToCharacter = selected != null
            && character.RelicPool.AllRelics.Any(r => r.GetType() == selected.GetType());

        if (SubclassRelicPicker.RandomMode || !belongsToCharacter)
        {
            SubclassRelicPicker.SelectRandomFor(character);
            SubclassRelicPicker.RandomMode = false;
            selected = SubclassRelicPicker.SelectedSubclassRelic;

            MainFile.Logger.Info(
                $"Rolled subclass {selected?.GetType().Name ?? "none"} for {character.Id}");
        }

        if (selected == null)
        {
            return;
        }

        async Task SwapRelic()
        {
           // it starts with the default one, so remove it. 
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
