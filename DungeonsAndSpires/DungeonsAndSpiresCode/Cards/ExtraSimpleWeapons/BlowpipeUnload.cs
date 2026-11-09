using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class BlowpipeUnload() : ExtraSimpleWeaponCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var played = new HashSet<CardModel>();

        // Darts draw cards, so darts can be pulled out of the draw pile mid-sequence; keep going until none are left.
        while (CombatState.HittableEnemies.Count > 0)
        {
            var dart = PileType.Hand.GetPile(Owner).Cards.FirstOrDefault(c => c is Dart && !played.Contains(c))
                ?? PileType.Draw.GetPile(Owner).Cards.FirstOrDefault(c => c is Dart && !played.Contains(c));

            if (dart == null)
            {
                break;
            }

            played.Add(dart);
            if (IsUpgraded && !dart.IsUpgraded)
            {
                CardCmd.Upgrade(dart);
            }

            var target = Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
            
            // TODO: this shit hella slow man ill fix this later i guess.........
            await CardCmd.AutoPlay(choiceContext, dart, target, AutoPlayType.Default, false, false);
        }
    }

    protected override void OnUpgrade()
    {
    }
}
