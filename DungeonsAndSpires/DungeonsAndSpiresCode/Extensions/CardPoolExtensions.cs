using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;

public static class CardPoolExtensions
{
    /// <summary>
    /// Combines the cards of two card pools, for use when lazily building a combined pool
    /// </summary>
    public static IEnumerable<CardModel> Add(this CardPoolModel pool, CardPoolModel other)
    {
        return pool.AllCards.Concat(other.AllCards);
    }
}