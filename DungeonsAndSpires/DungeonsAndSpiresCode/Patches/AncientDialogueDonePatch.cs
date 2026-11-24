using System.Reflection;
using DungeonsAndSpires.DungeonsAndSpiresCode.Ancients;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(NEventRoom), nameof(NEventRoom.OptionButtonClicked))]
internal static class AncientDialogueDonePatch
{
    private static readonly FieldInfo EventField = AccessTools.Field(typeof(NEventRoom), "_event");

    [HarmonyPostfix]
    private static void Postfix(NEventRoom __instance, EventOption option)
    {
        if (option.IsLocked)
        {
            return;
        }

        if (EventField.GetValue(__instance) is not AncientEventModel ancient)
        {
            return;
        }

        var runState = RunManager.Instance.State;
        if (runState == null)
        {
            return;
        }

        foreach (var model in runState.IterateHookListeners(null).OfType<IOnAncientDialogueDone>())
        {
            var abstractModel = (AbstractModel)(object)model;
            TaskHelper.RunSafely(model.OnAncientDialogueDone(ancient));
            abstractModel.InvokeExecutionFinished();
        }
    }
}
