using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class TrueStrikePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (Amount > 0 && dealer == Owner && isValid(cardSource))
        {
            return 1.5m;
        }
        return 1m;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!isValid(cardPlay.Card))
        {
            return;
        }

        await PowerCmd.Decrement(this);
    }

    private bool isValid(CardModel card)
    {
        if (card.Type == CardType.Attack && card.Tags.Any(x => x == DASCoreCardtags.Cantrip || x == DASCoreCardtags.Weapon))
        {
            return true;
        }
        return false;
    }
}