using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class Sanctuary() : SpellCard(1, 2, CardType.Power, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar(1),
        new BlockVar(20, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = PileType.Hand.GetPile(Owner).Cards
            .Where(c => !c.Keywords.Contains(CardKeyword.Retain))
            .ToList();
        if (hand.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-SANCTUARY.selectionPrompt"), DynamicVars.Cards.IntValue);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, prefs);
        var cards = selected.ToList();
        if (cards.Count == 0)
        {
            return;
        }

        var existing = Owner.Creature.GetPower<SanctuaryPower>();
        if (existing != null)
        {
            existing.AddRetainedCards(cards);
        }
        else
        {
            var power = await PowerCmd.Apply<SanctuaryPower>(choiceContext, Owner.Creature, DynamicVars.Block.IntValue, Owner.Creature, this);
            power?.AddRetainedCards(cards);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}