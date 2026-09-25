using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

public static class PotencyCmd
{
    public static event Action<Player>? OnChanged;

    public static async Task Add(PlayerChoiceContext context, Player player, int amount)
    {
        var state = player.PlayerCombatState?.GetPotencyState();
        if (state == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        state.Current += amount;

        MainFile.Logger.Info($"Potency gained: +{amount}, current: {state.Current}");

        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
            await PotencyHook.OnGained(combatState, context, player, amount);
    }

    public static async Task Remove(PlayerChoiceContext context, Player player, int amount)
    {
        var state = player.PlayerCombatState?.GetPotencyState();
        if (state == null || amount <= 0)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        int before = state.Current;
        state.Current = Math.Max(0, state.Current - amount);
        int removed = before - state.Current;

        MainFile.Logger.Info($"Potency lost: -{removed}, current: {state.Current}");

        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        //if (combatState != null)
            // TODO: onRemove? 
    }
}