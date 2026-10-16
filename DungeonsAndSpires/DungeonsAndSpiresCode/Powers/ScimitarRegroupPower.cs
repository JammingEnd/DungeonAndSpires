using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ScimitarRegroupPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _triggeredThisTurn;

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner.Player)
        {
            _triggeredThisTurn = false;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (_triggeredThisTurn || power.Owner != Owner || power is not DexterityPower || amount <= 0)
        {
            return;
        }

        _triggeredThisTurn = true;
        await PowerCmd.Apply<ScimitarRegroupDexterityPower>(choiceContext, Owner, Amount, Owner, null);
    }
}
