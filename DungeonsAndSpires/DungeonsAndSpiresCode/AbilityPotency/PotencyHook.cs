using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

public static class PotencyHook
{
    public static async Task OnGained(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        int amount)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IOnPotencyGained>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.OnPotencyGained(choiceContext, player, amount);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }

    public static decimal ModifyPotencyGain(ICombatState combatState, Player player, decimal originalAmount,
        ValueProp props, CardModel? cardSource,
        out IEnumerable<AbstractModel> modifiers)
    {
        modifiers = [];
        return originalAmount;
    }
}