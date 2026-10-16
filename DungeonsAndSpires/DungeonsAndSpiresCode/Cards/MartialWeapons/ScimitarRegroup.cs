using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class ScimitarRegroup() : MartialWeaponCard(3, CardType.Power, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.Finesse];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<ScimitarRegroupPower>("Extra", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ScimitarRegroupPower>(choiceContext, Owner.Creature, DynamicVars["Extra"].IntValue, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ScimitarRegroupPower>()
    ];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
