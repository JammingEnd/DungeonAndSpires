using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class WallOfWater() : SpellCard(3, 2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Exhaust,
        CoreKeywords.Ritual
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            decimal frostbite = enemy.GetPowerAmount<FrostbitePower>();
            if (frostbite > 0)
            {
                await PowerCmd.Apply<FrostbitePower>(choiceContext, enemy, frostbite, Owner.Creature, this);
            }

            decimal shocked = enemy.GetPowerAmount<ShockedPower>();
            if (shocked > 0)
            {
                await PowerCmd.Apply<ShockedPower>(choiceContext, enemy, shocked, Owner.Creature, this);
            }

            decimal poison = enemy.GetPowerAmount<PoisonPower>();
            if (poison > 0)
            {
                await PowerCmd.Apply<PoisonPower>(choiceContext, enemy, poison, Owner.Creature, this);
            }
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FrostbitePower>(),
        HoverTipFactory.FromPower<ShockedPower>(),
        HoverTipFactory.FromPower<PoisonPower>()
    ];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
