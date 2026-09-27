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


public class VitriolicSphere() : SpellCard(6, 2, CardType.Skill, CardRarity.Rare, TargetType.AllEnemies)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Poison", (4, 2) },
        { "Damage", (4, 5) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<VulnerablePower>("Vulnerable", 3),
        new PowerVar<PoisonPower>("Poison", 5),
        new IntVar("Damage", 10)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.HittableEnemies.ToArray();

        await PowerCmd.Apply<VulnerablePower>(choiceContext, enemies, DynamicVars["Vulnerable"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<PoisonPower>(choiceContext, enemies, DynamicVars["Poison"].GetCalculatedValue(), Owner.Creature, this);
        await PowerCmd.Apply<VitriolicSpherePower>(choiceContext, enemies, DynamicVars["Damage"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<VitriolicSpherePower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["PoisonBase"].UpgradeValueBy(2m);
        DynamicVars["PoisonExtra"].UpgradeValueBy(1m);
        DynamicVars["DamageBase"].UpgradeValueBy(4m);
        DynamicVars["DamageExtra"].UpgradeValueBy(2m);
    }
}