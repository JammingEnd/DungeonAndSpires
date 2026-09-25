using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;

/// <summary>
/// Vertical list of spellslot blocks shown on the left edge of the combat screen. One row per
/// spell level (level 1 at the bottom), showing a Roman numeral and the slots remaining, which
/// turn red when they hit 0.
/// </summary>
public partial class DASSpellslotUI : Control
{
    private static readonly string[] RomanNumerals = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };

    private static readonly Color NormalColor = new(1f, 1f, 1f);
    private static readonly Color EmptyColor = new(1f, 0.2f, 0.2f);

    private const float RowHeight = 30f;

    private Player? _player;
    private readonly Dictionary<int, Label> _labels = new();

    public void Initialize(Player player)
    {
        _player = player;

        var state = player.PlayerCombatState?.GetSpellslotsState();
        if (state == null)
        {
            return;
        }

        // Highest level at the top, level 1 at the bottom.
        float y = 0f;
        foreach (var (level, slot) in state.Spellslots.OrderByDescending(kv => kv.Key))
        {
            var label = new Label
            {
                Position = new Vector2(0f, y)
            };
            y += RowHeight;
            _labels[level] = label;
            AddChild(label);
            UpdateLabel(level, slot);
        }

        Size = new Vector2(80f, y);

        SpellslotsCmd.OnChanged += OnSpellslotsChanged;
    }

    private void UpdateLabel(int level, Spellslot slot)
    {
        if (!_labels.TryGetValue(level, out var label))
        {
            return;
        }

        string roman = level >= 1 && level <= RomanNumerals.Length ? RomanNumerals[level - 1] : level.ToString();
        label.Text = $"{roman} [{slot.GetCurrent()}]";
        label.Modulate = slot.GetCurrent() <= 0 ? EmptyColor : NormalColor;
    }

    private void OnSpellslotsChanged(Player player)
    {
        if (!GodotObject.IsInstanceValid(this))
        {
            SpellslotsCmd.OnChanged -= OnSpellslotsChanged;
            return;
        }

        if (_player == null || player != _player)
        {
            return;
        }

        var state = _player.PlayerCombatState?.GetSpellslotsState();
        if (state == null)
        {
            return;
        }

        foreach (var (level, slot) in state.Spellslots)
        {
            UpdateLabel(level, slot);
        }
    }
}