using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// Discarded exhaust cards grow back into the draw pile, one energy costlier.
public class SlingshotRegrowthPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner != Owner.Player || !card.Keywords.Contains(CardKeyword.Exhaust))
        {
            return;
        }

        var copy = card.CombatState?.CloneCard(card);
        if (copy == null)
        {
            return;
        }

        copy.EnergyCost.AddThisCombat(1);
        copy.InvokeEnergyCostChanged();
        await CardPileCmd.Add(copy, PileType.Draw, CardPilePosition.Random);
        CardCmd.Preview(copy);
        await PowerCmd.Decrement(this);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        await PowerCmd.Remove(this);
    }
}
