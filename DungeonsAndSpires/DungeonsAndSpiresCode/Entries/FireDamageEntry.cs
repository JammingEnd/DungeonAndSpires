using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.CombatHistoryEntries;

public class FireDamageEntry(
    Creature actor,
    int roundNumber,
    CombatSide currentSide,
    CombatHistory history,
    IEnumerable<Player> players) : CombatHistoryEntry(actor, roundNumber, currentSide, history, players)
{
    public override string Description
    {
        get
        {
            string id = Actor.Player != null ? Actor.Player.Character.Id.Entry : Actor.Monster.Id.Entry;
            return $"{id} dealt fire damage";
        }
    }
}