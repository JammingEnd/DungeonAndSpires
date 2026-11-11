using BaseLib.Abstracts;
using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Rogue;

public class DaggerThroat() : ExtraSimpleWeaponCard(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy), ITranscendenceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Finesse];
    
    public override decimal FinesseStrength => 3;

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(4, ValueProp.Move),
    ];
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
       return ModelDb.Get<FirstAndLast>();
    }
}