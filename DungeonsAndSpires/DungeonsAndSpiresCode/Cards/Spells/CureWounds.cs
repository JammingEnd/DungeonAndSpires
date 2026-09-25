using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class CureWounds() : SpellCard(0, 1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar("Discard", 1),
        new CardsVar("Draw", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int discard = DynamicVars["Discard"].IntValue;

        var hand = PileType.Hand.GetPile(Owner).Cards.ToList();
        if (hand.Count == 0)
        {
            return;
        }
        bool HasStatusses = PileType.Hand.GetPile(Owner).Cards.Any(c => c.Type == CardType.Status || c.Type == CardType.Curse);
        if (HasStatusses)
        {
            discard *= 2;
        }

        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 1, discard);
        var selected = await CardSelectCmd.FromSimpleGrid(choiceContext, hand, Owner, prefs);
        foreach (var card in selected)
        {
            await CardCmd.Discard(choiceContext, selected);
        }
        
        if(HasStatusses)
            await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Discard"].UpgradeValueBy(1);
        DynamicVars["Draw"].UpgradeValueBy(1);
    }
}