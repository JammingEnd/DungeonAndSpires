using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortbowTelekinesisPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        for (int i = 0; i < Amount; i++)
        {
            await PlayRandomFromHand(choiceContext, player,
                c => c.Tags.Contains(DASCoreCardtags.Cantrip) || c.Tags.Contains(DASCoreCardtags.Bow));
        }
    }

    private async Task PlayRandomFromHand(PlayerChoiceContext choiceContext, Player player, Func<CardModel, bool> filter)
    {
        var candidates = PileType.Hand.GetPile(player).Cards.Where(filter).ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        var card = player.RunState.Rng.CombatCardSelection.NextItem(candidates);
        if (card == null)
        {
            return;
        }

        Creature? target = null;
        if (card.TargetType != TargetType.Self && card.TargetType != TargetType.None)
        {
            var enemies = CombatState.HittableEnemies.ToList();
            if (enemies.Count == 0)
            {
                return;
            }
            target = player.RunState.Rng.CombatTargets.NextItem(enemies);
        }

        if (card.TargetType == TargetType.Self)
            target = Owner;

        await CardCmd.AutoPlay(choiceContext, card, target);
    }
}