using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Rogue;

// finess scaling increases every time 
//TODO: idk think of condition
public class FirstAndLast() : ExtraSimpleWeaponCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Finesse];
    public override decimal FinesseStrength => DynamicVars["scaling"].IntValue;
    
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new IntVar("scaling", 4)
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }
    
    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}