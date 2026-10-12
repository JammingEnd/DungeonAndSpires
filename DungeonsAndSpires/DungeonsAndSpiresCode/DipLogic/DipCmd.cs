using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public static class DipCmd
{
    public static event Action<Player>? OnChanged;

    // Fired whenever a specific card's dip changed, so self-managing DipInfoControl can update.
    public static event Action<CardModel>? OnDipChanged;

    public static async Task Add(PlayerChoiceContext context, Player player, CardModel card, DipInfo dipInfo)
    {
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null || dipInfo == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        state.WeaponDips[card] = dipInfo;

        OnDipChanged?.Invoke(card);
        OnChanged?.Invoke(player);

        var combatState = player.Creature.CombatState;
        if (combatState != null)
        {

            await DipHook.OnChanged(combatState, context, player, card, dipInfo);
        }
    }

    public static async Task Use(PlayerChoiceContext context, Player player, CardModel card)
    {
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;
        if(!state.WeaponDips.ContainsKey(card))
            return;

        if (state.WeaponDips[card].DecreaseDuration())
        {
            state.WeaponDips.Remove(card);
        }

        OnDipChanged?.Invoke(card);
    }
    public static async Task IncreaseDuration(PlayerChoiceContext context, Player player, CardModel card, int amount)
    {
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;
        if(!state.WeaponDips.ContainsKey(card))
            return;

       state.WeaponDips[card].IncreaseDuration(amount);

       OnDipChanged?.Invoke(card);
    }
    public static async Task ModifyStrength(PlayerChoiceContext context, Player player, CardModel card, int amount)
    {
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;
        if(!state.WeaponDips.ContainsKey(card))
            return;

        state.WeaponDips[card].ModifyStrength(amount);

        OnDipChanged?.Invoke(card);
    }

    public static async Task Remove(PlayerChoiceContext context, Player player, CardModel card, DipInfo dipInfo)
    {
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null || dipInfo == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        state.WeaponDips.Remove(card);

        OnDipChanged?.Invoke(card);
        OnChanged?.Invoke(player);
        
        var combatState = player.Creature.CombatState;
        if (combatState != null)
            await DipHook.OnChanged(combatState, context, player, card, dipInfo);
    }
}