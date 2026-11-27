using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ArmorOfAgathysPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private CardModel? _cardSource;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this && cardSource != null)
        {
            _cardSource = cardSource;
        }
    }

    public override async Task BeforeDamageReceived(PlayerChoiceContext choiceContext, Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        CardModel? source = _cardSource;
        if (target != Owner || dealer == null || source == null)
        {
            return;
        }
        if (Owner.Block <= 0)
        {
            return;
        }
        if (!props.IsPoweredAttack())
        {
            return;
        }

        await DamageCmd.Attack(Amount).FromCard(source, null).Targeting(dealer).Unpowered().Execute(choiceContext);
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await PowerCmd.Remove(this);
    }
}
