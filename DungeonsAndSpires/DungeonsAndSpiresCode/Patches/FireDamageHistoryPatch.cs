using DungeonsAndSpires.DungeonsAndSpiresCode.CombatHistoryEntries;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(CombatHistory), nameof(CombatHistory.DamageReceived))]
internal class FireDamageHistoryPatch
{
    [HarmonyPostfix]
    private static void Postfix(ICombatState combatState, Creature receiver, Creature? dealer, CardModel? cardSource)
    {
        if (cardSource == null || !cardSource.Keywords.Contains(CoreKeywords.Fire))
        {
            return;
        }

        var addMethod = AccessTools.Method(typeof(CombatHistory), "Add", [typeof(ICombatState), typeof(CombatHistoryEntry)]);
        addMethod.Invoke(CombatManager.Instance.History, [
            combatState,
            new FireDamageEntry(dealer ?? receiver, combatState.RoundNumber, combatState.CurrentSide, CombatManager.Instance.History, combatState.Players)
        ]);
    }
}