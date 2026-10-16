using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency.UI;

public class AddPotency
{
    private static readonly AddedNode<NEnergyCounter, Control> _addedNode = new(
        "res://DungeonsAndSpires/Scenes/Core/potency_control.tscn",
        (nEnergyCounter, display) =>
        {
            var player = AccessTools.Field(typeof(NEnergyCounter), "_player").GetValue(nEnergyCounter) as Player;
            if (player == null)
            {
                return;
            }

            // The Sorcerer always shows Potency; other characters reveal it once they have some.
            bool alwaysVisible = player.Character.CardPool is SorcererCardPool;

            var label = display.GetNodeOrNull<Label>("potencyLabel");
            if (label == null)
            {
                return;
            }

            void Refresh()
            {
                int potency = player.PlayerCombatState?.GetPotency() ?? 0;
                label.Text = potency.ToString();
                if (!alwaysVisible)
                {
                    display.Visible = potency > 0;
                }
            }

            void OnPotencyChanged(Player changedPlayer)
            {
                if (!GodotObject.IsInstanceValid(label))
                {
                    PotencyCmd.OnChanged -= OnPotencyChanged;
                    return;
                }
                if (changedPlayer == player)
                {
                    Refresh();
                }
            }

            PotencyCmd.OnChanged += OnPotencyChanged;
            Refresh();

            var hoverTip = new HoverTip(
                new LocString("static_hover_tips", "DUNGEONSANDSPIRES-POTENCY.title"),
                new LocString("static_hover_tips", "DUNGEONSANDSPIRES-POTENCY.description"));
            display.MouseEntered += () =>
                NHoverTipSet.CreateAndShow(display, hoverTip)?.SetGlobalPosition(display.GlobalPosition + new Vector2(-70f, -200f));
            display.MouseExited += () => NHoverTipSet.Remove(display);

            nEnergyCounter.AddChild(display);
        });
}