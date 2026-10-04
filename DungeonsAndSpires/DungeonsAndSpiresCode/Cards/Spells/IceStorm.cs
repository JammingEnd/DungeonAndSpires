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


public class IceStorm() : SpellCard(3, 2, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Cold
    ];

    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Damage", (4, 5) },
        { "Frostbite", (4, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(11, ValueProp.Unpowered),
        new PowerVar<FrostbitePower>("Frostbite", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.HittableEnemies.ToArray();
        await DamageCmd.Attack(DynamicVars["Damage"].GetCalculatedValue()).FromCard(this, cardPlay).TargetingAllOpponents(CombatState).Execute(choiceContext);
        await PowerCmd.Apply<FrostbitePower>(choiceContext, enemies, DynamicVars["Frostbite"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<FrostbitePower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DamageBase"].UpgradeValueBy(4m);
        DynamicVars["DamageExtra"].UpgradeValueBy(2m);
        DynamicVars["FrostbiteBase"].UpgradeValueBy(1m);
    }
}