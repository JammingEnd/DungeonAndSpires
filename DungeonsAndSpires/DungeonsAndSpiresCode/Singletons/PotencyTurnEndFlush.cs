using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Rooms;

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

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if(combatState.RoundNumber > 1)
            return;
        
        if (combatState == null)
        {
            return;
        }
        // theres some cross-encounter stuff going on.
        foreach (var player in combatState.Players)
        {
            await PotencyCmd.Reset(player);
        }
    }
}