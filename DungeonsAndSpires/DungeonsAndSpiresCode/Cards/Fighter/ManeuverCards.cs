using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;

// what is a maneuver.. in dnd its something extra to put on an attack or reaction... 
// since these are, until act3, played on turn start, it must act that way

[Pool(typeof(TokenCardPool))]
public abstract class ManeuverCard() : CoreCard(0, CardType.None, CardRarity.Token, TargetType.None)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust, CardKeyword.Retain];
}

// draw 2 cards
public class BaitAndSwitch() : ManeuverCard()
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext, 2, Owner);
    }
}

// gain 2 energy,
public class QuickToss() : ManeuverCard()
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(2, Owner);
    }
}

// gain 6 vigor
public class MenacingAttack() : ManeuverCard()
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, 6, Owner.Creature, this);
    }
}
