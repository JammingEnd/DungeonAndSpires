using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class PsychicLance() : SpellCard(4, 2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Thunder
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(5, ValueProp.Unpowered),
        new IntVar("Hits", 3),
        new IntVar("BonusHits", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hits = (int)DynamicVars["Hits"].GetCalculatedValue();
        if (cardPlay.Target.GetPowerAmount<ShockedPower>() > 0)
        {
            hits += (int)DynamicVars["BonusHits"].GetCalculatedValue();
        }

        decimal damage = DynamicVars["Damage"].GetCalculatedValue();
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target).WithHitCount(hits).Execute(choiceContext);

        await PowerCmd.Remove<ShockedPower>(cardPlay.Target);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Thunder),
        HoverTipFactory.FromPower<ShockedPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["BonusHits"].UpgradeValueBy(1);
    }
}