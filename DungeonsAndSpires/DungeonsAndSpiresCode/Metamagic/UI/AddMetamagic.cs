using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic.UI;

public class AddMetamagic
{
    private static readonly AddedNode<NEnergyCounter, Control> _addedNode = new(
        "res://DungeonsAndSpires/Scenes/Core/metamagic_control.tscn",
        (nEnergyCounter, display) =>
        {
            // NEnergyCounter._player is private; grab it via reflection.
            var player = AccessTools.Field(typeof(NEnergyCounter), "_player").GetValue(nEnergyCounter) as Player;
            if (player == null)
            {
                return;
            }

            var label = display.GetNodeOrNull<Label>("metamagicLabel");
            if (label == null)
            {
                return;
            }

            void Refresh()
            {
                label.Text = player.PlayerCombatState?.GetMetamagic().ToString() ?? "0";
            }

            void OnMetamagicChanged(Player changedPlayer)
            {
                if (!GodotObject.IsInstanceValid(label))
                {
                    MetamagicCmd.OnChanged -= OnMetamagicChanged;
                    return;
                }
                if (changedPlayer == player)
                {
                    Refresh();
                }
            }

            MetamagicCmd.OnChanged += OnMetamagicChanged;
            Refresh();

            var hoverTip = new HoverTip(
                new LocString("static_hover_tips", "DUNGEONSANDSPIRES-METAMAGIC.title"),
                new LocString("static_hover_tips", "DUNGEONSANDSPIRES-METAMAGIC.description"));
            display.MouseEntered += () =>
                NHoverTipSet.CreateAndShow(display, hoverTip)?.SetGlobalPosition(display.GlobalPosition + new Vector2(-70f, -200f));
            display.MouseExited += () => NHoverTipSet.Remove(display);

            nEnergyCounter.AddChild(display);
        });
}