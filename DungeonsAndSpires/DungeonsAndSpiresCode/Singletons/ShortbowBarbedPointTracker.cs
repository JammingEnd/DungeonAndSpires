using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

public class ShortbowBarbedPointTracker() : CustomSingletonModel(HookType.Combat)
{
    private readonly Dictionary<Player, List<(ShortbowBarbedPoint card, bool ready)>> _pending = new();

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card is ShortbowBarbedPoint barb)
        {
            AddPending(cardPlay.Player, barb);
        }

        if (!cardPlay.Card.Tags.Contains(DASCoreCardtags.Weapon) || cardPlay.Card.Tags.Contains(DASCoreCardtags.Bow))
        {
            return;
        }

        if (!_pending.TryGetValue(cardPlay.Player, out var list))
        {
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            var (card, ready) = list[i];
            if (ready && card.Pile?.Type == PileType.Discard)
            {
                await CardPileCmd.Add(card, PileType.Hand);
                // Consume so it isn't returned again on later attacks this turn.
                list[i] = (card, false);
            }
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy)
        {
            return;
        }

        foreach (var kv in _pending)
        {
            var list = kv.Value;
            for (int i = 0; i < list.Count; i++)
            {
                var (card, ready) = list[i];
                if (ready)
                {
                    list.RemoveAt(i);
                    i--;
                }
                else
                {
                    list[i] = (card, true);
                }
            }
        }
    }

    private void AddPending(Player player, ShortbowBarbedPoint card)
    {
        if (!_pending.TryGetValue(player, out var list))
        {
            list = new List<(ShortbowBarbedPoint, bool)>();
            _pending[player] = list;
        }
        if (!list.Any(e => e.card == card))
        {
            list.Add((card, false));
        }
    }
}