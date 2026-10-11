using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public static class DipHook
{
    public static async Task OnChanged(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        CardModel card, DipInfo dipInfo)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IOnDipChanged>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.OnDipChanged(choiceContext, player, card, dipInfo);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }
}