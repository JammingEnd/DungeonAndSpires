using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

[Pool(typeof(TokenCardPool))]
public class Scrappie() : ExtraSimpleWeaponCard(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Retain, CardKeyword.Exhaust];

    private static readonly SpireField<CardModel, Player?> Giver = new((CardModel _) => null);

    public static void SetGiver(CardModel card, Player giver) => Giver[card] = giver;

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(1, ValueProp.Move),
        new IntVar("Increase", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is Dart && cardPlay.Card.Owner == Giver[this])
        {
            DynamicVars.Block.BaseValue += DynamicVars["Increase"].IntValue;
        }

        return Task.CompletedTask;
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Increase"].UpgradeValueBy(1);
    }
}
