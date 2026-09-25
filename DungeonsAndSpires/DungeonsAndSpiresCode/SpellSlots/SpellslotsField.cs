using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;

public static class SpellslotsField
{
    public static readonly SpireField<PlayerCombatState, SpellslotsState> State = new(() => null); 
}