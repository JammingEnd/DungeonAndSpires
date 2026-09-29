using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

public static class MetamagicField
{
    public static readonly SpireField<PlayerCombatState, MetamagicState> State = new(() => null);
}