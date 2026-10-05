using DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;
using MegaCrit.Sts2.addons.mega_text;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(NCharacterSelectScreen))]
internal static class CharacterSelectSubclassPatch
{
    private static SubclassRelicPicker? _picker;

    [HarmonyPatch("_Ready")]
    [HarmonyPostfix]
    private static void ReadyPostfix(NCharacterSelectScreen __instance)
    {
        _picker = new SubclassRelicPicker
        {
            Name = "SubclassRelicPicker",
            AnchorLeft = 0,
            AnchorTop = 0,
            AnchorRight = 0,
            AnchorBottom = 0,
            OffsetLeft = 220,
            OffsetTop = 720,
            OffsetRight = 810,
            OffsetBottom = 850
        };
        _picker.Visible = false;
        _picker.RelicSelected += relic => UpdateDisplayedRelic(__instance, relic);
        __instance.AddChild(_picker);
    }

    [HarmonyPatch("SelectCharacter")]
    [HarmonyPostfix]
    private static void SelectCharacterPostfix(CharacterModel characterModel)
    {
        _picker?.UpdateFor(characterModel);
    }

    private static void UpdateDisplayedRelic(NCharacterSelectScreen screen, SubclassRelic relic)
    {
        var title = screen.GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/Relic/Name/RichTextLabel");
        var description = screen.GetNode<MegaRichTextLabel>("InfoPanel/VBoxContainer/Relic/Description");
        var icon = screen.GetNode<TextureRect>("InfoPanel/VBoxContainer/Relic/Icon");
        var outline = screen.GetNode<TextureRect>("InfoPanel/VBoxContainer/Relic/Icon/Outline");

        title.Text = relic.Title.GetFormattedText();
        description.Text = relic.DynamicDescription.GetFormattedText();
        icon.Texture = relic.Icon;
        outline.Texture = relic.IconOutline;
        icon.SelfModulate = Colors.White;
        outline.SelfModulate = StsColors.halfTransparentBlack;
    }
}