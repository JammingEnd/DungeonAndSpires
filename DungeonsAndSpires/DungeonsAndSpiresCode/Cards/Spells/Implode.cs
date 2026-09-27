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


public class Implode() : SpellCard(6, 1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Damage", (1, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Unpowered),
        new IntVar("FrostbiteBonus", 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal baseDamage = DynamicVars["Damage"].GetCalculatedValue();
        decimal bonusPerStack = DynamicVars["FrostbiteBonus"].GetCalculatedValue();

        foreach (var enemy in CombatState!.HittableEnemies)
        {
            decimal total = baseDamage + bonusPerStack * enemy.GetPowerAmount<FrostbitePower>();
            await CreatureCmd.Damage(choiceContext, enemy, total, ValueProp.Unpowered, Owner.Creature, this, cardPlay);
            await PowerCmd.Remove<FrostbitePower>(enemy);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<FrostbitePower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["FrostbiteBonus"].UpgradeValueBy(2m);
    }
}