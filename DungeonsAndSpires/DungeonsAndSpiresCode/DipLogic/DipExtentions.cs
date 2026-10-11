using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;

public static class DipExtentions
{
    private static readonly CardKeyword[] possibleTypes = new[]
    {
        CoreKeywords.Cold,
        CoreKeywords.Fire,
        CoreKeywords.Lightning,
        CoreKeywords.Poison,
        CoreKeywords.Thunder
    };
    public static DipInfo GenerateRandomDip(int turns, Player player, int strength)
    {
        var type = possibleTypes[player.Creature.CombatState.RunState.Rng.CombatCardGeneration.NextInt(possibleTypes.Length)];
        DipInfo newDip = new DipInfo(type, strength, turns);
        return newDip;
    }
    
    public static DipState? GetDipState(this PlayerCombatState state)
    {
        return DipField.State[state];
    }

    public static DipInfo? GetDip(this PlayerCombatState state, CardModel card)
    {
        var dips = state.GetDipState();
        return dips?.WeaponDips.TryGetValue(card, out var dip) == true ? dip : null;
    }

    public static bool HasDip(this CardModel card)
    {
        return card.Owner.PlayerCombatState.GetDipState().WeaponDips.ContainsKey(card);
    }
}