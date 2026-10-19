using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class GreatswordPoise() : MartialWeaponCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.TwoHanded, CardKeyword.Exhaust];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<GreatswordTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Strength", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, DynamicVars["Strength"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<GreatswordPoisePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<GreatswordPoisePower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(1);
    }
}
