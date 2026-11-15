using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

/// <summary>
/// Implemented by relics/powers that can stop a spell from leaving an Arcane Exhaustion behind
/// (e.g. Ghost Gloves while Hidden). Return false to veto the status card.
/// </summary>
public interface IShouldCreateExhaustion
{
    bool ShouldCreateExhaustion(SpellCard spell, PlayerChoiceContext choiceContext, Player player);
}

public static class SpellExhaustionHook
{
    public static bool ShouldCreate(ICombatState? combatState, SpellCard card, PlayerChoiceContext choiceContext, Player player)
    {
        if (!card.ShouldCreateExhaustion(choiceContext, player))
        {
            return false;
        }

        if (combatState == null)
        {
            return true;
        }

        foreach (var listener in combatState.IterateHookListeners().OfType<IShouldCreateExhaustion>())
        {
            if (!listener.ShouldCreateExhaustion(card, choiceContext, player))
            {
                return false;
            }
        }

        return true;
    }
}
