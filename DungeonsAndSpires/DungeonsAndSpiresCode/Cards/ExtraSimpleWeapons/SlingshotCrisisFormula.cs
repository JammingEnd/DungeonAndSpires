using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotCrisisFormula() : ExtraSimpleWeaponCard(2, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<StrengthPower>("Strength", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, -DynamicVars["Strength"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<SlingshotCrisisFormulaPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>(),
        DipTips.Dip,
        HoverTipFactory.Static(StaticHoverTip.ReplayDynamic, new DynamicVar("Times", 1))
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(-1m);
    }
}
