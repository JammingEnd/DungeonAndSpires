using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class DaggerOffhandPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private CardModel? _sourceCard;

    public void SetSourceCard(CardModel card)
    {
        _sourceCard = card;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (Amount <= 0 || cardPlay.Card == _sourceCard || cardPlay.Card.Owner != Owner.Player || !cardPlay.Card.Keywords.Contains(CoreKeywords.Finesse))
        {
            return;
        }

        await PowerCmd.Remove(this);
        await CardCmd.AutoPlay(choiceContext, cardPlay.Card, cardPlay.Target);
    }
}