using MegaCrit.Sts2.Core.Entities.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class MaceFarcryPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public void GrantDiscount()
    {
        foreach (var player in Owner.CombatState.Players.Where(p => p != Owner.Player))
        {
            var hand = player.PlayerCombatState?.Hand.Cards;
            if (hand == null)
            {
                continue;
            }

            var pool = hand.ToList();
            for (int i = 0; i < Amount && pool.Count > 0; i++)
            {
                var card = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(pool);
                pool.Remove(card);
                card.EnergyCost.AddThisTurnOrUntilPlayed(-1);
                card.InvokeEnergyCostChanged();
            }
        }
    }
}
