using System.Reflection;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

internal static class NonModCardTagging
{
    private static readonly FieldInfo TagsField = AccessTools.Field(typeof(CardModel), "_tags");
    
    public static void TagWeaponIfForeign(CardModel? card)
    {
        if (card == null || card is CoreCard)
        {
            return;
        }

        var tags = card.Tags.ToHashSet();
        if (tags.Add(DASCoreCardtags.Weapon))
        {
            TagsField.SetValue(card, tags);
        }
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.AfterCreated))]
internal static class NonModGeneratedCardPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel __instance) => NonModCardTagging.TagWeaponIfForeign(__instance);
}

[HarmonyPatch(typeof(CardPileCmd), nameof(CardPileCmd.Add),
    new[] { typeof(CardModel), typeof(PileType), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool) })]
internal static class NonModDeckAddPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel card, PileType newPileType)
    {
        if (newPileType == PileType.Deck)
        {
            NonModCardTagging.TagWeaponIfForeign(card);
        }
    }
}


[HarmonyPatch(typeof(RunState), nameof(RunState.AddCard),
    new[] { typeof(CardModel), typeof(Player) })]
internal static class NonModRunAddCardPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel card) => NonModCardTagging.TagWeaponIfForeign(card);
}