using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.HoverTips), MethodType.Getter)]
internal static class OffCharacterWeaponTooltipPatch
{
    private static IHoverTip? _tip;

    private static IHoverTip Tip => _tip ??= new HoverTip(
        new LocString("static_hover_tips", "DUNGEONSANDSPIRES-OFFCHARACTER_WEAPON.title"),
        new LocString("static_hover_tips", "DUNGEONSANDSPIRES-OFFCHARACTER_WEAPON.description"));

    [HarmonyPostfix]
    private static void Postfix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (!AddToCard(__instance))
        {
            return;
        }

        __result = __result.Append(Tip);
    }

    private static bool AddToCard(CardModel card)
    {
        if (card.IsCanonical)
        {
            return false;
        }

        var player = card.Owner;
        if (player?.Character == null || IsModCharacter(player.Character))
        {
            return false;
        }
        
        if (!player.Deck.Cards.Any(c => c is CoreCard))
        {
            return false;
        }
        
        return card.Description.GetRawText().Contains("weapon", StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsModCharacter(CharacterModel character)
    {
        
        return character.GetType().Namespace?.StartsWith("DungeonsAndSpires", StringComparison.Ordinal) == true;
    }
}
