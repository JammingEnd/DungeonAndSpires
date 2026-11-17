using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityLogic;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

// Tracks per-combat ability charges for IAbilityCast characters (reset every combat).
public class AbilityTracker : CustomSingletonModel
{
    private static readonly Dictionary<Player, int> Charges = new();

    public AbilityTracker() : base(HookType.Combat)
    {
        CombatManager.Instance.CombatSetUp += OnCombatSetUp;
    }

    private void OnCombatSetUp(CombatState state)
    {
        Charges.Clear();
        foreach (var player in state.Players)
        {
            Charges[player] = MaxChargesFor(player);
        }
    }

    private static int MaxChargesFor(Player player) => player.Character is IAbilityCast ability ? ability.MaxCharges : 0;

    public static int GetCharges(Player player) => Charges.TryGetValue(player, out var charges) ? charges : 0;

    public static bool SpendCharge(Player player)
    {
        if (!Charges.TryGetValue(player, out var charges) || charges <= 0)
        {
            return false;
        }

        Charges[player] = charges - 1;
        return true;
    }
}
