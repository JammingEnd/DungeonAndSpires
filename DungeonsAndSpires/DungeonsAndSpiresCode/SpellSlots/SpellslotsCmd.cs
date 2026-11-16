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

        int oldAmount = slot.GetCurrent();
        slot.Add(amount);
        int currentAmount = slot.GetCurrent();
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
        {
            await SpellslotsHook.OnGained(combatState, context, player, level, amount);
            await SpellslotsHook.OnAmountChanged(combatState, context, player, level, oldAmount, currentAmount);
        }
    }

    public static async Task ConsumeSpellSlotForLevel(PlayerChoiceContext context, Player player, int level, int amount = 1)
    {
        var state = player.PlayerCombatState?.GetSpellslotsState();
        var slot = player.PlayerCombatState?.GetSpellslotForLevel(level);
        if (state == null || slot == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        int oldAmount = slot.GetCurrent();
        slot.Consume(amount);
        int currentAmount = slot.GetCurrent();
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
        {
            await SpellslotsHook.OnConsumed(combatState, context, player, level, amount);
            await SpellslotsHook.OnAmountChanged(combatState, context, player, level, oldAmount, currentAmount);
        }
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
    
    /// <summary>
    /// Change the max spellslots someone has, also refills said spellslots, so use it only at the start of combat
    /// </summary>
    public static async Task IncreaseMaxSpellslots(PlayerChoiceContext context, Player player, int level, int amount)
    {
        var state = player.PlayerCombatState?.GetSpellslotsState();
        if (state == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;
        state.Spellslots[level].IncreaseMax(amount);
        state.Spellslots[level].Refill();
        OnChanged?.Invoke(player);
    }
    public static async Task IncreaseMaxSpellslots(PlayerChoiceContext context, Player player, float multiplier)
    {
        var state = player.PlayerCombatState?.GetSpellslotsState();
        if (state == null || multiplier <= 1)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;
        foreach (var (_, value) in state.Spellslots)
        {
            int newMax = (int)Math.Round(value.GetMax() * multiplier);
            value.SetMax(newMax);
            value.Refill();
        }
        OnChanged?.Invoke(player);
    }
}