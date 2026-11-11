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

public class MaceEchoingBlow() : ExtraSimpleWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Bludgeoning];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new IntVar("Resonance", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var target = cardPlay.Target;
        bool primed = target.GetPowerAmount<FrostbitePower>() > 0 || target.GetPowerAmount<ShockedPower>() > 0 || target.GetPowerAmount<Reverberation>() > 0;

        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        if (primed)
        {
            await PowerCmd.Apply<ResonancePower>(choiceContext, Owner.Creature, DynamicVars["Resonance"].IntValue, Owner.Creature, this);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FrostbitePower>(),
        HoverTipFactory.FromPower<ShockedPower>(),
        HoverTipFactory.FromPower<Reverberation>(),
        HoverTipFactory.FromPower<ResonancePower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
        DynamicVars["Resonance"].UpgradeValueBy(1);
    }
}
