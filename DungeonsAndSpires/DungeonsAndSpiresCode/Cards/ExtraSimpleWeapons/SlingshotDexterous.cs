using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotDexterous() : ExtraSimpleWeaponCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<DexterityPower>("Dexterity", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, DynamicVars["Dexterity"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<SlingshotDexterousPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<DexterityPower>()
    ];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
