using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class PoisonSpray() : SpellCard(0, 1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    protected override HashSet<CardTag> CanonicalTags => [DASCoreCardtags.Cantrip];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new PotencyVar(),
        new IntVar("PotencyStep", 3m),
        ..MakeCalculatedVar("Damage", 3, static (model, creature) => Math.Floor(model.Owner.PlayerCombatState.GetPotency() / 3m), 3),
        ..MakeCalculatedVar("Poison", 2, static (model, creature) => Math.Floor(model.Owner.PlayerCombatState.GetPotency() / 3m), 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var enemies = CombatState!.HittableEnemies.ToArray();
        if (enemies.Length == 0)
            return;

        await CreatureCmd.Damage(choiceContext, enemies, DynamicVars["Damage"].GetCalculatedValue(), ValueProp.Unpowered, Owner.Creature);
        await PowerCmd.Apply<PoisonPower>(choiceContext, enemies, DynamicVars["Poison"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DamageExtra"].UpgradeValueBy(2m);
        DynamicVars["PoisonExtra"].UpgradeValueBy(1m);
    }
}