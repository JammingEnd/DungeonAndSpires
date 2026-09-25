using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;

public static class SpellslotsCmd
{
    public static event Action<Player>? OnChanged;

    public static async Task AddSpellSlotForLevel(PlayerChoiceContext context, Player player, int level, int amount = 1)
    {
        var state = player.PlayerCombatState?.GetSpellslotsState();
        var slot = player.PlayerCombatState?.GetSpellslotForLevel(level);
        if (state == null || slot == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        slot.Add(amount);
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
            await SpellslotsHook.OnGained(combatState, context, player, level, amount);
    }

    public static async Task ConsumeSpellSlotForLevel(PlayerChoiceContext context, Player player, int level, int amount = 1)
    {
        var state = player.PlayerCombatState?.GetSpellslotsState();
        var slot = player.PlayerCombatState?.GetSpellslotForLevel(level);
        if (state == null || slot == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        slot.Consume(amount);
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
            await SpellslotsHook.OnConsumed(combatState, context, player, level, amount);
    }
    
    public static Task SetMaxForLevel(PlayerChoiceContext context, Player player, int level, int max)
    {
        var slot = player.PlayerCombatState?.GetSpellslotForLevel(level);
        if (slot == null)
            return Task.CompletedTask;

        if (CombatManager.Instance.IsOverOrEnding)
            return Task.CompletedTask;

        slot.SetMax(max);
        OnChanged?.Invoke(player);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Restores slots on the highest levels that are currently missing, used by warlock
    /// </summary>
    public static async Task RestoreHighestMissingSlots(PlayerChoiceContext context, Player player, int amount)
    {
        var state = player.PlayerCombatState?.GetSpellslotsState();
        if (state == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        var combatState = player.Creature.CombatState;
        foreach (var (level, slot) in state.Spellslots.OrderByDescending(slot => slot.Key))
        {
            if (amount <= 0)
                break;

            int missing = slot.GetMax() - slot.GetCurrent();
            int restore = Math.Min(amount, missing);
            if (restore > 0)
            {
                slot.Add(restore);
                amount -= restore;
                if (combatState != null)
                    await SpellslotsHook.OnGained(combatState, context, player, level, restore);
            }
        }

        OnChanged?.Invoke(player);
    }
}