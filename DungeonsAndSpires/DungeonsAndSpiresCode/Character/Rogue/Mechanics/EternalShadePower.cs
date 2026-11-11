using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class EternalShadePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public Task GrantFreeCard(PlayerChoiceContext choiceContext)
    {
        var hand = Owner.Player?.PlayerCombatState?.Hand.Cards.ToList();
        if (hand == null || hand.Count == 0)
        {
            return Task.CompletedTask;
        }

        var card = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(hand);
        card.EnergyCost.SetThisTurnOrUntilPlayed(0);
        card.InvokeEnergyCostChanged();

        return Task.CompletedTask;
    }
}
