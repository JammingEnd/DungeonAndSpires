using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

// Rage: halve block gained, halve incoming enemy damage, grow +1 damage per raging turn (Amount).
// Powers spend 1; missing an Attack ends it.
public class BarbarianRagePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private bool _playedAttackThisTurn;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _playedAttackThisTurn = false;
        return Task.CompletedTask;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await PowerCmd.ModifyAmount(choiceContext, this, 1, Owner, null);
        _playedAttackThisTurn = false;
    }
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || cardSource == null || target == Owner)
        {
            return 0;
        }

        return Amount;
    }
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target == Owner && dealer != null && !dealer.IsPlayer)
        {
            return 0.5m;
        }

        return 1m;
    }
    
    public override decimal ModifyBlockMultiplicative(Creature target, decimal block, ValueProp props, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (props == ValueProp.Unpowered)
            return 1m;
        return target == Owner ? 0.5m : 1m;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player)
        {
            return;
        }

        if (cardPlay.Card.Type == CardType.Attack)
        {
            _playedAttackThisTurn = true;
        }
        else if (cardPlay.Card.Type == CardType.Power)
        {
            // Hopium: paying a Power cost while Weak sheds Weak instead of Rage.
            if (Owner.Player.GetRelic<HopiumRelic>() != null && Owner.GetPowerAmount<WeakPower>() > 0)
            {
                await PowerCmd.Apply<WeakPower>(choiceContext, Owner, -1, Owner, null);
            }
            else
            {
                await PowerCmd.Decrement(this);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || _playedAttackThisTurn)
        {
            return;
        }

        await PowerCmd.Remove(this);
    }
    
    public override async Task AfterRemoved(Creature oldOwner)
    {
        await PowerCmd.Remove<BearSpirit>(oldOwner);
        await PowerCmd.Remove<EagleSpirit>(oldOwner);
        await PowerCmd.Remove<WolfSpirit>(oldOwner);
        await PowerCmd.Remove<ElkSpirit>(oldOwner);
        if (oldOwner.IsPlayer)
        {
            oldOwner.Player.GetRelic<TotemWarriorRelic>().SetGrantedSpirit(false);
        }
    }
}
