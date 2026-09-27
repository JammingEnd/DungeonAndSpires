using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class LightningBolt() : SpellCard(3, 1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Lightning
    ];

    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Damage", (3, 4) },
        { "Shocked", (3, 2) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(9, ValueProp.Unpowered),
        new PowerVar<ShockedPower>("Shocked", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.HittableEnemies.ToArray();
        await CreatureCmd.Damage(choiceContext, enemies, DynamicVars["Damage"].GetCalculatedValue(), ValueProp.Unpowered, Owner.Creature, this, cardPlay);
        await PowerCmd.Apply<ShockedPower>(choiceContext, enemies, DynamicVars["Shocked"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<ShockedPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DamageBase"].UpgradeValueBy(4m);
        DynamicVars["DamageExtra"].UpgradeValueBy(2m);
        DynamicVars["ShockedBase"].UpgradeValueBy(1m);
        DynamicVars["ShockedExtra"].UpgradeValueBy(1m);
    }
}