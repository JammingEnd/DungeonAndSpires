using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class QuarterstaffHighgroundPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private class Data
    {
        public bool HadBlock;
    }

    protected override object InitInternalData() => new Data();

    // Block is cleared after this, so remember whether we started with any.
    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        GetInternalData<Data>().HadBlock = side == CombatSide.Player && participants.Contains(Owner) && Owner.Block > 0;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var data = GetInternalData<Data>();
        if (!data.HadBlock)
        {
            return;
        }

        data.HadBlock = false;
        await PlayerCmd.GainEnergy(Amount, Owner.Player);
    }
}
