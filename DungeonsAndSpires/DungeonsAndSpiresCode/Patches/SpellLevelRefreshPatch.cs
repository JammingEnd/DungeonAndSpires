using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(NCard), "Reload")]
internal class SpellLevelRefreshPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCard __instance)
    {
        var display = __instance.GetNodeOrNull<SpellLevelControl>("%CardContainer/SpellLevel");
        if (display == null)
        {
            return;
        }
        
        var label = display.GetNodeOrNull<Label>("Label");
        if (label == null)
        {
            return;
        }

        if (__instance.Model is SpellCard spell)
        {
            label.Text = AddLevelToCards.ToNumeral(spell.Level);
            display.Visible = true;
        }
        else
        {
            display.Visible = false;
        }
    }
}