using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal class NCombatUiSpellslotPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCombatUi __instance, CombatState state)
    {
        var me = LocalContext.GetMe(state);
        if (me == null || me.Character is not SorcererCharacter)
        {
            return;
        }

        var ui = new DASSpellslotUI
        {
            Name = "SpellslotUI"
        };
        __instance.AddChild(ui);
        ui.SetAnchorsPreset(Control.LayoutPreset.CenterLeft);
        ui.Position = new Vector2(8f, 360f);
        ui.Initialize(me);
    }
}