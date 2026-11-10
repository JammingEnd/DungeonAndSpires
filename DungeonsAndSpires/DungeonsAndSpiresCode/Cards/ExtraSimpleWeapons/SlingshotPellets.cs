using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotPellets() : ExtraSimpleWeaponCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar(4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int draws = DynamicVars.Cards.IntValue;
        
        // yeah so i gotta shuffle if i draw more than in the draw pile
        if (PileType.Draw.GetPile(Owner).Cards.Count < draws && PileType.Discard.GetPile(Owner).Cards.Count > 0)
        {
            await CardPileCmd.Shuffle(choiceContext, Owner);
        }

        for (int i = 0; i < draws; i++)
        {
            var drawPile = PileType.Draw.GetPile(Owner);
            if (drawPile.Cards.Count == 0)
            {
                await CardPileCmd.ShuffleIfNecessary(choiceContext, Owner);
                drawPile = PileType.Draw.GetPile(Owner);
                if (drawPile.Cards.Count == 0)
                {
                    break;
                }
            }

            if (PileType.Hand.GetPile(Owner).Cards.Count < CardPile.MaxCardsInHand)
            {
                await CardPileCmd.Draw(choiceContext, 1, Owner);
                continue;
            }

            await CardCmd.AutoPlay(choiceContext, drawPile.Cards[0], null);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(2);
    }
}
