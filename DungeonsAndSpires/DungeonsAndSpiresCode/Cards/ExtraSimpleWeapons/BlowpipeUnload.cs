using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class BlowpipeUnload() : ExtraSimpleWeaponCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var darts = PileType.Hand.GetPile(Owner).Cards.Where(c => c is Dart)
            .Concat(PileType.Draw.GetPile(Owner).Cards.Where(c => c is Dart))
            .ToList();

        if (darts.Count == 0 || CombatState.HittableEnemies.Count == 0)
        {
            return;
        }

        if (IsUpgraded)
        {
            foreach (var dart in darts)
            {
                if (!dart.IsUpgraded)
                {
                    CardCmd.Upgrade(dart);
                }
            }
        }

        foreach (var dart in darts)
        {
            // A previous dart may have drawn or played this one already.
            if (dart.Pile?.Type is not (PileType.Hand or PileType.Draw))
            {
                continue;
            }

            var target = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            await CardCmd.AutoPlay(choiceContext, dart, target);
        }
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Dart>()),
    ];

    protected override void OnUpgrade()
    {
    }
}
