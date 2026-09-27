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

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class WallOfFire() : SpellCard(4, 2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "HitDamage", (4, 2) },
        { "EndDamage", (4, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("HitDamage", 4),
        new IntVar("EndDamage", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<WallOfFirePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        power?.SetAmounts(
            DynamicVars["HitDamageBase"].BaseValue, DynamicVars["HitDamageExtra"].BaseValue,
            DynamicVars["EndDamageBase"].BaseValue, DynamicVars["EndDamageExtra"].BaseValue);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["HitDamageBase"].UpgradeValueBy(2m);
        DynamicVars["HitDamageExtra"].UpgradeValueBy(1m);
        DynamicVars["EndDamageBase"].UpgradeValueBy(1m);
        DynamicVars["EndDamageExtra"].UpgradeValueBy(1m);
    }
}