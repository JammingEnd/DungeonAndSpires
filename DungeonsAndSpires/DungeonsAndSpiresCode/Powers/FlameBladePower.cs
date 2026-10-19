using System.Collections.Generic;
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

public class FlameBladePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        if (card.Owner != Owner.Player)
            return false;
        if (card.Type != CardType.Attack)
            return false;
        if (!card.Tags.Contains(DASCoreCardtags.Weapon))
            return false;
        return keywords.Add(CoreKeywords.Fire);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner != Owner.Player || card.Type != CardType.Attack || !card.Tags.Contains(DASCoreCardtags.Weapon))
        {
            return;
        }

        await DamageCmd.Attack(Amount).FromCard(card, cardPlay).Targeting(cardPlay.Target).Unpowered().Execute(choiceContext);

        if (cardPlay.Target != null)
        {
            var frostbite = cardPlay.Target.GetPower<FrostbitePower>();
            if (frostbite != null)
            {
                await PowerCmd.Decrement(frostbite);
            }
        }
    }
}