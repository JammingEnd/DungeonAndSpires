using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

public static class PotencyExtentions
{
    public static PotencyState? GetPotencyState(this PlayerCombatState state)
    {
        return PotencyField.State[state];
    }

    public static int GetPotency(this PlayerCombatState state)
    {
        var potency = state.GetPotencyState();
        return potency?.Current ?? 0;
    }
}