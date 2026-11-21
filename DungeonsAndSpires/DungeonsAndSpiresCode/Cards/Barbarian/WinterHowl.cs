using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Barbarian;
[Pool(typeof(BarbarianCardPool))]
public class WinterHowl() : SimpleWeaponCard(0, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new PowerVar<FrostbitePower>("Frostbite", 3)
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Cold, CoreKeywords.Bludgeoning];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await PowerCmd.Apply<FrostbitePower>(choiceContext, cardPlay.Target, DynamicVars["Frostbite"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<WinterHowlPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FrostbitePower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["Frostbite"].UpgradeValueBy(2m);
    }
}
