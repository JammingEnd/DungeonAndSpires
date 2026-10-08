using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class Divination() : SpellCard(4, 2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("AbilityPotency", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PotencyCmd.Add(choiceContext, Owner, DynamicVars["AbilityPotency"].IntValue);

        var drawPile = PileType.Draw.GetPile(Owner).Cards.Where(x => x.Type is not CardType.Status or CardType.Curse or CardType.Quest).ToList();
        if (drawPile.Count == 0)
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            var card = Owner.RunState.Rng.CombatCardSelection.NextItem(drawPile);
            drawPile.Remove(card);
            if (card != null)
            {
                CardCmd.Upgrade(card, CardPreviewStyle.HorizontalLayout);
                CardCmd.Preview(card);
            }
        }
        
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars["AbilityPotency"].UpgradeValueBy(3m);
    }
}