using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class GreatswordOppressivePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _chained;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player)
        {
            return;
        }

        bool twoHandedAttack = cardPlay.Card.Type == CardType.Attack
            && cardPlay.Card is MartialWeaponCard martial && martial.TwoHandedPower != null;

        if (!twoHandedAttack)
        {
            _chained = false;
            await PowerCmd.Remove<GreatswordOppressiveStrengthPower>(Owner);
            return;
        }

        if (_chained)
        {
            await PowerCmd.Apply<GreatswordOppressiveStrengthPower>(choiceContext, Owner, Amount, Owner, null);
        }

        _chained = true;
    }
}
