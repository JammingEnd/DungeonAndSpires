using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;

public static class SpellslotsHook
{
    public static async Task OnGained(ICombatState combatState, PlayerChoiceContext choiceContext, Player player, int level, int amount)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IOnSpellSlotGained>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.OnSpellSlotGained(choiceContext, player, level, amount);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }

    public static async Task OnConsumed(ICombatState combatState, PlayerChoiceContext choiceContext, Player player, int level, int amount)
    {
        foreach (var model in combatState.IterateHookListeners().OfType<IOnSpellSlotConsumed>())
        {
            var abstractModel = (AbstractModel)(object)model;
            choiceContext.PushModel(abstractModel);
            await model.OnSpellSlotConsumed(choiceContext, player, level, amount);
            abstractModel.InvokeExecutionFinished();
            choiceContext.PopModel(abstractModel);
        }
    }
}