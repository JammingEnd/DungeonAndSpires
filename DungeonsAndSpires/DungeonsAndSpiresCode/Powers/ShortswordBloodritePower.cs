using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortswordBloodritePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power.Owner != Owner || power is not StrengthPower)
            return;
        if (amount <= 0)
            return;

        var cardInHand = Owner.Player.PlayerCombatState.Hand.Cards;
        var randomCard = Owner.CombatState.RunState.Rng.CombatCardSelection.NextItem(cardInHand);
        if (randomCard == null || randomCard.EnergyCost.GetResolved() <= 0)
            return;
        randomCard.EnergyCost.AddThisTurnOrUntilPlayed(-1);
    }
}