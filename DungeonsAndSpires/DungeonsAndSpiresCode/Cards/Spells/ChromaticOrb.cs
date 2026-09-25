using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class ChromaticOrb() : SpellCard(1, 1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{

    public override IEnumerable<CardKeyword> ElementOptions =>
    [
        CoreKeywords.Fire,
        CoreKeywords.Cold,
        CoreKeywords.Thunder,
        CoreKeywords.Lightning,
        CoreKeywords.Acid,
        CoreKeywords.Poison
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(10, ValueProp.Unpowered),
        new PowerVar<PoisonPower>("PoisonPower", 2),
        new PowerVar<VulnerablePower>("VulnerablePower", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ChooseElement(choiceContext);
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await ApplyImbuedElementEffect(choiceContext, cardPlay.Target);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["PoisonPower"].UpgradeValueBy(2m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
    }
}