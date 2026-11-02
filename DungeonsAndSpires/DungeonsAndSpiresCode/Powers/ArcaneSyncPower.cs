using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// At the end of your turn, each ally's card plays give you a spellslot of that level.
public class ArcaneSyncPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var combatState = Owner.CombatState;
        var player = Owner.Player;
        if (combatState == null || player == null)
        {
            return;
        }

        foreach (var ally in combatState.Players.Where(p => p != player))
        {
            int played = CombatManager.Instance.History.CardPlaysFinished
                .Count(entry => entry.HappenedThisTurn(combatState) && entry.Actor == ally.Creature);

            if (played <= 0)
            {
                continue;
            }

            await SpellslotsCmd.AddSpellSlotForLevel(choiceContext, player, Math.Min(played, 9));
        }
    }
}
