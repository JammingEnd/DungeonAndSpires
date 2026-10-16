using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;
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
        DipField.State[__instance] = new DipState();
        PotencyField.State[__instance] = new PotencyState();
        WeaponPlayField.State[__instance] = new WeaponPlayState();

        AssignForSorcerer(player, __instance);
        AssignForFighter(player, __instance);
    }

    private static void AssignForSorcerer(Player player, PlayerCombatState __instance)
    {
        bool IsSorcerer = player.Character.CardPool is SorcererCardPool;
        
        if (IsSorcerer)
        {
            MetamagicField.State[__instance] = new MetamagicState();
        }
        else
        {
            return;
        }
        SpellslotsField.State[__instance] = new SpellslotsState();
    }

    private static void AssignForFighter(Player player, PlayerCombatState __instance)
    {
        bool isFighter = player.Character.CardPool is FighterCardPool;
        if (!isFighter)
            return;
        bool isEldritch = player.Relics.Any(r => r is EldritchKnightRelic);
        if (isEldritch)
        {
            SpellslotsField.State[__instance] = new SpellslotsState(true);
        }
    }
}