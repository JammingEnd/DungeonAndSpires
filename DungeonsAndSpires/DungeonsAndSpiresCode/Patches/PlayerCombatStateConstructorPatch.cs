using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(PlayerCombatState), MethodType.Constructor)]
[HarmonyPatch([typeof(Player)])]
internal class PlayerCombatStateConstructorPatch
{
    [HarmonyPostfix]
    private static void Postfix(Player player, PlayerCombatState __instance)
    {
        AssignForSorcerer(player, __instance);
    }

    private static void AssignForSorcerer(Player player, PlayerCombatState __instance)
    {
        bool isHalfCaster = player.Character.CardPool is not SorcererCardPool;
        SpellslotsField.State[__instance] = new SpellslotsState(isHalfCaster);

        // Potency and Metamagic are Sorcerer-only.
        if (!isHalfCaster)
        {
            PotencyField.State[__instance] = new PotencyState();
            MetamagicField.State[__instance] = new MetamagicState();
        }
    }
}