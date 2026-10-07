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
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class Moonbeam() : SpellCard(2, 2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Damage", (2, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(4, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<MoonbeamPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        power?.SetAmounts(
            DynamicVars["DamageBase"].BaseValue, DynamicVars["DamageExtra"].BaseValue,
            HeightenedLevelBonus(PotencyVars["Damage"].step));
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<MoonbeamPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DamageBase"].UpgradeValueBy(3m);
    }
}