using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

// okay so im not making upgrades subclass relics, instead imma give them a relic based on their subclass
[HarmonyPatch(typeof(TouchOfOrobas), nameof(TouchOfOrobas.AfterObtained))]
internal static class TouchOfOrobasKeepRelicPatch
{
    [HarmonyPrefix]
    private static bool Prefix(TouchOfOrobas __instance, ref Task __result)
    {
        var owner = __instance.Owner;
        if (owner == null)
        {
            return true;
        }

        var replacement = owner.Relics
            .OfType<DungeonsAndSpiresRelic>()
            .Select(relic => relic.GetUpgradeReplacement())
            .FirstOrDefault(candidate => candidate != null);

        if (replacement == null)
        {
            return true;
        }

        __result = Grant(replacement, owner);
        return false;
    }

    private static async Task Grant(RelicModel replacement, Player owner)
    {
        await RelicCmd.Obtain(replacement.ToMutable(), owner);
    }
}
