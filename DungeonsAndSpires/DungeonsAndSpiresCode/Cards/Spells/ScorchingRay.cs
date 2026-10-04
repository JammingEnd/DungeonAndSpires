using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class ScorchingRay() : SpellCard(2, 2, CardType.Attack, CardRarity.Uncommon, TargetType.RandomEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Fire
    ];

    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Hits", (3, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Hits", 4),
        new DamageVar(4, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hits = (int)DynamicVars["Hits"].GetCalculatedValue();
        decimal damage = DynamicVars["Damage"].IntValue;

        var enemies = CombatState!.HittableEnemies.ToArray();
        for (int i = 0; i < hits; i++)
        {
            if (enemies.Length == 0)
            {
                break;
            }
            var enemy = Owner.RunState.Rng.CombatTargets.NextItem<Creature>(enemies);
            if (enemy != null)
            {
                await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
            }
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}