using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class MaulEarthshakerPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // after playing a two-handed skill, a random enemy gains frostbite
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        if (cardPlay.Card.Type == CardType.Skill && cardPlay.Card.Keywords.Contains(CoreKeywords.TwoHanded))
        {
            var enemy = Owner.CombatState.RunState.Rng.CombatTargets.NextItem(Owner.CombatState.HittableEnemies);
            if (enemy != null)
            {
                await PowerCmd.Apply<FrostbitePower>(choiceContext, enemy, Amount, Owner, null);
                await PowerCmd.Apply<Reverberation>(choiceContext, enemy, Amount, Owner, null);
            }
        }
    }
}
