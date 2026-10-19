using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
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
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CardsVar("amount", 1)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["amount"].UpgradeValueBy(1);
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CardPileCmd.Draw(choiceContext,  DynamicVars["amount"].IntValue, Owner);
    }
}

// gain 2 energy,
public class QuickToss() : ManeuverCard()
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new EnergyVar("amount", 1)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["amount"].UpgradeValueBy(1);
    }
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        await PlayerCmd.GainEnergy( DynamicVars["amount"].IntValue, Owner);
    }
}

// Apply 1 vulnerable
public class MenacingAttack() : ManeuverCard()
{
    public override TargetType TargetType => TargetType.AnyEnemy;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PowerVar<VulnerablePower>("amount", 1)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["amount"].UpgradeValueBy(1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
       
        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars["amount"].IntValue, Owner.Creature, this);
    }
}
