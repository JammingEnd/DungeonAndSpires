using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

// Temporary potency is flushed back to the base at turn end.
public class PotencyTurnEndFlush() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var combatState = participants.FirstOrDefault()?.CombatState;
        if (combatState == null)
        {
            return;
        }

        foreach (var player in combatState.Players)
        {
            await PotencyCmd.FlushTurnTemp(choiceContext, player);
        }
    }
}