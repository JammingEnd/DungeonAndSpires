using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;

public static class SpellslotsExtentions
{
    public static SpellslotsState? GetSpellslotsState(this PlayerCombatState state)
    {
        return SpellslotsField.State[state];
    }

    public static Spellslot? GetSpellslotForLevel(this PlayerCombatState state, int level)
    {
        var slots = state.GetSpellslotsState();
        return slots != null && slots.Spellslots.TryGetValue(level, out var slot) ? slot : null;
    }

    public static bool HasAvailableSlotForLevel(this PlayerCombatState state, int level)
    {
        var slot = state.GetSpellslotForLevel(level);
        return slot != null && slot.GetCurrent() > 0;
    }
}