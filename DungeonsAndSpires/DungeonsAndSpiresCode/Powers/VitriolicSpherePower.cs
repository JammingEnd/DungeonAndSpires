using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class VitriolicSpherePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var context = new ThrowingPlayerChoiceContext();
        await CreatureCmd.Damage(context, Owner, Amount, ValueProp.Unpowered, Applier, null, null);
        await PowerCmd.Remove(this);
    }
}