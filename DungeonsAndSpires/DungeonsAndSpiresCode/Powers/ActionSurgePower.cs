using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ActionSurgePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _grantPending;
    private int _remaining;

    private bool OnExtraTurn => Owner.Player != null && CombatManager.Instance.PlayersTakingExtraTurn.Contains(Owner.Player);

    public override int DisplayAmount => _remaining;
    
    public void Grant()
    {
        _grantPending = true;
        _remaining = Amount;
        InvokeDisplayAmountChanged();
    }

    public override bool ShouldTakeExtraTurn(Player player)
    {
        return _grantPending && player == Owner.Player;
    }

    public override bool ShouldPlay(CardModel card, AutoPlayType autoPlayType)
    {
        return !(OnExtraTurn && _remaining <= 0);
    }
    
    public override bool ShouldDraw(Player player, bool fromHandDraw)
    {
        if (fromHandDraw)
        {
            return true;
        }
        return !(player == Owner.Player && OnExtraTurn);
    }

    public override Task AfterTakingExtraTurn(Player player)
    {
        if (player == Owner.Player)
        {
            _grantPending = false;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (OnExtraTurn && cardPlay.Player == Owner.Player)
        {
            _remaining = Math.Max(0, _remaining - 1);
            InvokeDisplayAmountChanged();
        }
        return Task.CompletedTask;
    }

    // Once the extra turn is over, drop the power.
    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player && OnExtraTurn)
        {
            await PowerCmd.Remove(this);
        }
    }
}
