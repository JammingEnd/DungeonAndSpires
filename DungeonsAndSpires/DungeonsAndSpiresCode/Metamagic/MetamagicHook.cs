using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

public static class MetamagicHook
{
    public static async Task OnGained(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        int amount)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IOnMetamagicGained>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.OnMetamagicGained(choiceContext, player, amount);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }

    public static async Task OnChanged(ICombatState combatState, PlayerChoiceContext choiceContext, Player player,
        int current, int currentAfterChange)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IOnMetamagicChanged>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.OnMetamagicChanged(choiceContext, player, current, currentAfterChange);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }

    public static decimal ModifyMetamagicGain(ICombatState combatState, Player player, decimal originalAmount,
        ValueProp props, CardModel? cardSource,
        out IEnumerable<AbstractModel> modifiers)
    {
        modifiers = [];
        return originalAmount;
    }
}