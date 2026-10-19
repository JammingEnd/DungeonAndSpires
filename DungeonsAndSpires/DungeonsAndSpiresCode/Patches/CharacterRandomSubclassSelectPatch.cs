using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Screens.CharacterSelect;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

[HarmonyPatch(typeof(NCharacterSelectScreen), "PlayerChanged")]
internal static class CharacterRandomSubclassSelectPatch
{
    [HarmonyPostfix]
    private static void Postfix(NCharacterSelectScreen __instance, StartRunLobbyPlayer player, bool isRandomCharacterResolution)
    {
        if (!isRandomCharacterResolution)
        {
            return;
        }

        var lobby = __instance.Lobby;
        if (lobby == null || lobby.LocalPlayer.id != player.id)
        {
            return;
        }

        if (player.character is not (SorcererCharacter or FighterCharacter))
        {
            return;
        }

        SubclassRelicPicker.SelectRandomFor(player.character);
    }
}
