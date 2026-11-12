using DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(NCharacterSelectScreen), "SelectCharacter")]
internal static class CharacterRandomSubclassSelectPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCharacterSelectButton charSelectButton)
    {
        if (charSelectButton.IsRandom)
        {
            SubclassRelicPicker.RandomMode = true;
            SubclassRelicPicker.ClearSelection();
            return;
        }

        SubclassRelicPicker.RandomMode = false;
    }
}
