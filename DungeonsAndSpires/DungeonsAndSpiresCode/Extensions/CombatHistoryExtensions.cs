using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;

public static class CombatHistoryExtensions
{
    private static readonly MethodInfo AddMethod =
        AccessTools.Method(typeof(CombatHistory), "Add", [typeof(ICombatState), typeof(CombatHistoryEntry)]);

    // CombatHistory.Add is private, so custom entries are recorded through reflection.
    public static void Record(this CombatHistory history, ICombatState combatState, CombatHistoryEntry entry) =>
        AddMethod.Invoke(history, [combatState, entry]);
}
