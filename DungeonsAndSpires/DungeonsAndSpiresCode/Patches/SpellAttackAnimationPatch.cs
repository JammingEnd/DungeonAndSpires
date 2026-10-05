using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

// override the default anim
[HarmonyPatch(typeof(AttackCommand), nameof(AttackCommand.FromCard))]
internal static class SpellAttackAnimationPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel card, AttackCommand __result)
    {
        if (card is SpellCard spell)
        {
            __result.WithAttackerAnim(spell.AttackAnimName, spell.Owner.Character.CastAnimDelay);
        }
    }
}
