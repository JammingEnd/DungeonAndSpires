using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

public static class MetamagicExtensions
{
    public static MetamagicState? GetMetamagicState(this PlayerCombatState state)
    {
        return MetamagicField.State[state];
    }

    public static int GetMetamagic(this PlayerCombatState state)
    {
        var metamagic = state.GetMetamagicState();
        return metamagic?.Current ?? 0;
    }
}