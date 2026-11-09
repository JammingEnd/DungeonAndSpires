using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class MaceCrownHead() : ExtraSimpleWeaponCard(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Bludgeoning];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(17, ValueProp.Move),
        new BlockVar(15, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        bool primed = target.GetPowerAmount<FrostbitePower>() > 0 || target.GetPowerAmount<Reverberation>() > 0;

        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        if (!primed)
        {
            return;
        }

        await CommonActions.CardBlock(this, cardPlay);
        EnergyCost.AddThisCombat(-1);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FrostbitePower>(),
        HoverTipFactory.FromPower<Reverberation>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(8m);
    }
}
