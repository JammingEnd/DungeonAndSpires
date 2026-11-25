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

    [HarmonyPrefix]
    private static bool Prefix(NEventRoom __instance, EventOption option)
    {
        if (option.IsLocked || !option.IsProceed)
        {
            return true;
        }

        if (EventField.GetValue(__instance) is not AncientEventModel ancient)
        {
            return true;
        }

        var runState = RunManager.Instance.State;
        if (runState == null)
        {
            return true;
        }
        
        if (!runState.IterateHookListeners(null).Any(l => l is IOnAncientDialogueDone))
        {
            return true;
        }

        TaskHelper.RunSafely(Dispatch(ancient, runState));
        return false;
    }

    private static async Task Dispatch(AncientEventModel ancient, RunState runState)
    {
        foreach (var model in runState.IterateHookListeners(null).OfType<IOnAncientDialogueDone>())
        {
            var abstractModel = (AbstractModel)(object)model;
            await model.OnAncientDialogueDone(ancient);
            abstractModel.InvokeExecutionFinished();
        }
        RunManager.Instance.Proceed();
    }
}
