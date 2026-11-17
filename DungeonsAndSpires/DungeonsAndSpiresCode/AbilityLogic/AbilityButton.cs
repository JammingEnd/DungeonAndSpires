using BaseLib.Utils;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityLogic;

public class AbilityButton
{
    private static readonly AddedNode<NEnergyCounter, Control> _addedNode = new(
        "res://DungeonsAndSpires/Scenes/Core/ability_button_control.tscn",
        (nEnergyCounter, display) =>
        {
            var player = AccessTools.Field(typeof(NEnergyCounter), "_player").GetValue(nEnergyCounter) as Player;
            if (player == null)
            {
                return;
            }
            
            if (player.Character is not IAbilityCast ability)
            {
                display.Visible = false;
                return;
            }

            var button = display.GetNodeOrNull<TextureButton>("button");
            var icon = display.GetNodeOrNull<TextureRect>("icon");
            var label = display.GetNodeOrNull<Label>("label");
            if (button == null || icon == null || label == null)
            {
                return;
            }

            void Refresh()
            {
                label.Text = ability.GetCharges(player).ToString();
                display.Visible = true;
            }

            var iconTexture = GD.Load<Texture2D>(ability.AbilityIconPath);
            if (iconTexture != null)
            {
                icon.Texture = iconTexture;
            }

            button.Pressed += () =>
            {
                ability.CastAbility(player);
                Refresh();
            };

            var hoverTip = new HoverTip(ability.AbilityTitle, ability.AbilityDescription);
            button.MouseEntered += () =>
            {
                button.SelfModulate = new Color(0.7f, 0.7f, 0.7f);
                NHoverTipSet.CreateAndShow(button, hoverTip)?.SetGlobalPosition(button.GlobalPosition + new Vector2(-70f, -200f));
            };
            button.MouseExited += () =>
            {
                button.SelfModulate = Colors.White;
                NHoverTipSet.Remove(button);
            };

            Refresh();
            nEnergyCounter.AddChild(display);
        });
}
