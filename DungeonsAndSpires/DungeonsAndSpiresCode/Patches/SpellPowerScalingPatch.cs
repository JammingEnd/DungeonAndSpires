using System.Threading.Tasks;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

// Spell attacks are ability damage: no Strength/Vigor, no Vigor consumption, unless stated otherwise.
public static class SpellPowerScaling
{
    public static bool IsAbilityDamage(CardModel? card)
    {
        return card is SpellCard spell && !spell.UsesPoweredAttack;
    }
}

[HarmonyPatch(typeof(StrengthPower), nameof(StrengthPower.ModifyDamageAdditive))]
internal static class SpellIgnoresStrengthPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel? cardSource, ref decimal __result)
    {
        if (SpellPowerScaling.IsAbilityDamage(cardSource))
        {
            __result = 0m;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(VigorPower), nameof(VigorPower.ModifyDamageAdditive))]
internal static class SpellIgnoresVigorPatch
{
    [HarmonyPrefix]
    private static bool Prefix(CardModel? cardSource, ref decimal __result)
    {
        if (SpellPowerScaling.IsAbilityDamage(cardSource))
        {
            __result = 0m;
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(VigorPower), nameof(VigorPower.BeforeAttack))]
internal static class SpellDoesNotConsumeVigorPatch
{
    [HarmonyPrefix]
    private static bool Prefix(AttackCommand command, ref Task __result)
    {
        if (SpellPowerScaling.IsAbilityDamage(command.ModelSource as CardModel))
        {
            __result = Task.CompletedTask;
            return false;
        }
        return true;
    }
}
