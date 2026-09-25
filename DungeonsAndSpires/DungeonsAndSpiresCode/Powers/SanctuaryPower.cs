using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class SanctuaryPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private class Data
    {
        public List<CardModel> RetainedCards { get; } = new();
        public bool PlayedAttack { get; set; }
    }

    protected override object InitInternalData() => new Data();

    public void AddRetainedCards(IEnumerable<CardModel> cards)
    {
        var data = GetInternalData<Data>();
        foreach (var card in cards)
        {
            if (data.RetainedCards.Contains(card))
            {
                continue;
            }
            data.RetainedCards.Add(card);
            card.AddKeyword(CardKeyword.Retain);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack && cardPlay.Card.Owner == Owner.Player)
        {
            GetInternalData<Data>().PlayedAttack = true;
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
            return;
    

        var data = GetInternalData<Data>();
        foreach (var card in data.RetainedCards)
        {
            card.RemoveKeyword(CardKeyword.Retain);
        }
        data.RetainedCards.Clear();

        await PowerCmd.Remove(this);
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if(side != CombatSide.Player)
            return;
        
        var data = GetInternalData<Data>();
        if (!data.PlayedAttack && Amount > 0)
        {
            await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Unpowered, null);
        }
    }
}