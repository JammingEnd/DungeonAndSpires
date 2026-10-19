using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class CrossbowLegendaryPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private int _attacksThisTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            _attacksThisTurn = 0;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player || cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }

        _attacksThisTurn++;
        if (_attacksThisTurn != 3)
        {
            return;
        }

        for (int i = 0; i < Amount; i++)
        {
            await CardCmd.AutoPlay(choiceContext, cardPlay.Card, cardPlay.Target);
        }
    }
}
