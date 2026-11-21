using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortbowPoisonArrowPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private CardModel? _sourceCard;

    public override Task AfterApplied(Creature? applier, CardModel? cardSource)
    {
        _sourceCard = cardSource;
        return Task.CompletedTask;
    }

    // 
    public override async Task AfterCardPlayedLate(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        var cardSource = cardPlay.Card;
        if (target != Owner)
            return;
        if (cardSource == _sourceCard)
            return;
        if(cardSource.Type != CardType.Attack)
            return;
        if(cardSource is SpellCard)
            return;
        if(!cardSource.Tags.Contains(DASCoreCardtags.Weapon))
            return;
        DipInfo newDip = DipExtentions.GenerateDip(CoreKeywords.Poison, Amount, 2);
        await DipCmd.Add(choiceContext, cardSource.Owner, cardSource, newDip);
        await PowerCmd.ModifyAmount(choiceContext, this, -Amount, null, null);
    }
}