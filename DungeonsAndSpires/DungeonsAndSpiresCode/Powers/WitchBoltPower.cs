using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class WitchBoltPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var player = participants.FirstOrDefault(c => c.Player != null)?.Player;
        if (player != null && player.PlayerCombatState != null && player.PlayerCombatState.Energy > 0)
        {
            await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unpowered, player.Creature);
        }

        await PowerCmd.Remove(this);
    }
}