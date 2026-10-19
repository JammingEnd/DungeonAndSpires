using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;

[Pool(typeof(FighterRelicPool))]
public class EchoKnightRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;

    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    private bool _echoNextAttack;
    private bool _copyNextCard;
    
    public override async Task BeforeCombatStart()
    {
        if (CurrentLevel < 2)
        {
            return;
        }

        var drawPile = Owner.PlayerCombatState?.DrawPile.Cards;
        if (drawPile == null || drawPile.Count == 0)
        {
            return;
        }

        var bottom = drawPile[drawPile.Count - 1];
        await CardCmd.AutoPlay(new BlockingPlayerChoiceContext(), bottom, null);
    }

    // Act 1: gaining Second Wind echoes your next attack.
    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (CurrentLevel >= 1 && amount > 0m && power is SecondWindPower && power.Owner == Owner.Creature)
        {
            _echoNextAttack = true;
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
        {
            return;
        }
        
        if (CurrentLevel >= 3 && cardPlay.Card is ActionSurge)
        {
            _copyNextCard = true;
            return;
        }

        if (_copyNextCard)
        {
            _copyNextCard = false;
            var combatState = Owner.Creature.CombatState;
            if (combatState != null)
            {
                var copy = combatState.CloneCard(cardPlay.Card);
                await CardPileCmd.Add(copy, PileType.Draw, CardPilePosition.Top);
            }
            return;
        }
        
        bool echo = _echoNextAttack && cardPlay.Card.Type == CardType.Attack;
        if (!echo)
        {
            return;
        }

        _echoNextAttack = false;
        await CardCmd.AutoPlay(choiceContext, cardPlay.Card, cardPlay.Target);
    }
}
