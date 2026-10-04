using DungeonsAndSpires.DungeonsAndSpiresCode.Relics;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;

public partial class SubclassRelicPicker : Control
{
    public static SubclassRelic? SelectedSubclassRelic { get; private set; }

    public event Action<SubclassRelic>? RelicSelected;

    private readonly List<(TextureButton button, SubclassRelic relic)> _options = new();

    //The dark backdrop container that holds the relic buttons.
    private readonly ColorRect _background = new()
    {
        Color = new Color(0f, 0f, 0f, 0.25f),
        MouseFilter = MouseFilterEnum.Ignore
    };

    public SubclassRelicPicker()
    {
        _background.SetAnchorsPreset(LayoutPreset.FullRect);
        AddChild(_background);
    }

    public void UpdateFor(CharacterModel character)
    {
        foreach (var child in _background.GetChildren())
        {
            _background.RemoveChild(child);
            child.QueueFree();
        }
        _options.Clear();
        SelectedSubclassRelic = null;

        var relics = character.RelicPool.AllRelics.OfType<SubclassRelic>().ToList();
        if (relics.Count == 0)
        {
            Visible = false;
            return;
        }

        Visible = true;
        float x = 0f;
        foreach (var relic in relics)
        {
            var icon = new TextureButton
            {
                Position = new Vector2(x, 0),
                Size = new Vector2(128, 128),
                TextureNormal = relic.Icon,
                TextureHover = relic.Icon,
                TexturePressed = relic.Icon,
                StretchMode = TextureButton.StretchModeEnum.KeepAspectCentered
            };

            var label = new Label
            {
                Position = new Vector2(x, 132),
                Size = new Vector2(128, 26),
                Text = relic.Title.GetFormattedText(),
                HorizontalAlignment = HorizontalAlignment.Center
            };

            var option = relic;
            icon.Pressed += () => Select(option);
            _background.AddChild(icon);
            _background.AddChild(label);
            _options.Add((icon, relic));
            x += 138f;
        }
    }

    private void Select(SubclassRelic relic)
    {
        SelectedSubclassRelic = relic;
        RelicSelected?.Invoke(relic);
        foreach (var (button, optionRelic) in _options)
        {
            button.Modulate = optionRelic == relic ? Colors.White : new Color(0.55f, 0.55f, 0.55f);
        }
    }
}