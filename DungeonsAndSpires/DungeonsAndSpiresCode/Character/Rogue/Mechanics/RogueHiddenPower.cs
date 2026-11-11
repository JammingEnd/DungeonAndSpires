using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class RogueHiddenPower : DungeonsAndSpiresPower
{
    public override PowerType Type  => PowerType.Buff;
    public override PowerStackType StackType  => PowerStackType.Single;

    private bool _isActive = true;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == Owner)
        {
            // Damage resistance? who knows
            return 0.8m;
        }
        if (dealer != Owner || cardPlay == null)
        {
            return 1m;
        }

        if (!_isActive)
            return 1m;

        return 1.5m;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner == Owner.Player)
        {
            _isActive = false;
        }

        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        _isActive = true;
        return Task.CompletedTask;
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner)
        {
            if (result.UnblockedDamage > 0)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}