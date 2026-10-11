using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public static class DipCmd
{
    public static event Action<Player>? OnChanged;

    public static async Task Add(PlayerChoiceContext context, Player player, CardModel card, DipInfo dipInfo)
    {
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null || dipInfo == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        state.WeaponDips[card] = dipInfo;

        card.SetUI(true);

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
            card.SetUI(false);
        }
        else
        {
            card.SetUI(true);
        }
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

       card.SetUI(true);
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

        card.SetUI(true);
    }

    public static async Task Remove(PlayerChoiceContext context, Player player, CardModel card, DipInfo dipInfo)
    {
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null || dipInfo == null)
            return;

        if (CombatManager.Instance.IsOverOrEnding)
            return;

        state.WeaponDips.Remove(card);

        card.SetUI(false);

        OnChanged?.Invoke(player);
        
        var combatState = player.Creature.CombatState;
        if (combatState != null)
            await DipHook.OnChanged(combatState, context, player, card, dipInfo);
    }

    internal static void SetUI(this CardModel card, bool enabled)
    {
        if (card == null)
            return;

        var display = DipField.UI[card];
        if (display == null || !GodotObject.IsInstanceValid(display))
            return;

        // Only weapon cards can hold a dip; otherwise hide the overlay.
        if (!card.Tags.Contains(DASCoreCardtags.Weapon))
        {
            display.Visible = false;
            return;
        }

        display.Visible = enabled;
        if (!enabled)
            return;

        var dip = card.Owner?.PlayerCombatState?.GetDip(card);
        if (dip == null)
        {
            display.Visible = false;
            return;
        }

        display.GetNodeOrNull<Label>("UseLabel")?.SetText(dip.UsesLeft.ToString());
        display.GetNodeOrNull<Label>("StrengthLabel")?.SetText(dip.Strength.ToString());

        var typeRect = display.GetNodeOrNull<TextureRect>("TypeRect");
        if (typeRect != null)
        {
            var icon = $"{MainFile.ResPath}/images/cards/dip_icons/{GetDipTypeIcon(dip.Type)}.png";
            var texture = GD.Load<Texture2D>(icon);
            if (texture != null)
            {
                typeRect.Texture = texture;
            }
        }
    }

    private static string GetDipTypeIcon(CardKeyword type)
    {
        if (type == CoreKeywords.Cold)
            return "cold";
        if (type == CoreKeywords.Fire)
            return "flamer";
        if (type == CoreKeywords.Lightning)
            return "lightning";
        if (type == CoreKeywords.Poison)
            return "poison";
        if (type == CoreKeywords.Thunder)
            return "thunder";
        if (type == CoreKeywords.Acid)
            return "acid";
        return "lightning";
    }
}