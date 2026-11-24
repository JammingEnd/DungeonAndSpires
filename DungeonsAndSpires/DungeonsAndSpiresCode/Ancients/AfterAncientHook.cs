using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Ancients;

public class AfterAncientHook : CustomSingletonModel
{
    public AfterAncientHook() : base(HookType.Run)
    {
    }

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room.ModelId is not { } id)
        {
            return;
        }

        // IMPORTANT: GetById/GetByIdOrNull<AncientEventModel> throw InvalidCastException for
        // non-ancient ids (they do a raw (T)value cast). Resolve as the base type, then type-check.
        if (ModelDb.GetByIdOrNull<AbstractModel>(id) is not AncientEventModel ancient)
        {
            return;
        }

        var runState = RunManager.Instance.State;
        if (runState == null)
        {
            return;
        }

        foreach (var model in runState.IterateHookListeners(null).OfType<IOnAncientVisited>())
        {
            await model.OnAncientVisited(ancient);
            ((AbstractModel)model).InvokeExecutionFinished();
        }
    }
}
