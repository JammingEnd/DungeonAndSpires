using System.Reflection;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Screens.CardLibrary;
using MegaCrit.Sts2.Core.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Patches;

// this is what fires on the compendium list
[HarmonyPatch(typeof(NCardLibrary), "ShowCardDetail")]
internal static class CompendiumCardClickedPatch
{
    private static readonly FieldInfo RunStateField = AccessTools.Field(typeof(NRun), "_state");

    [HarmonyPostfix]
    private static async void Postfix(NCardHolder holder)
    {
        var card = holder?.CardModel;
        if (card == null || NRun.Instance == null)
        {
            return;
        }
        
        // just to be sure, since how it works is it is a fire and forget interaction
        try
        {
            var runState = RunStateField.GetValue(NRun.Instance) as RunState;
            
            // might be scuffed in MP
            var player = runState?.Players.FirstOrDefault(p => p.Creature.Powers.Any(x => x is WishPower));
            if (player == null)
            {
                return;
            }

            var created = runState.CreateCard(ModelDb.GetById<CardModel>(card.Id), player);
            if(created.Rarity == CardRarity.Ancient || created.Rarity == CardRarity.Curse ||  created.Rarity == CardRarity.Status)
                return;
            created.EnergyCost.SetThisTurn(0);
            await CardPileCmd.AddGeneratedCardToCombat(created, PileType.Hand, player);
            var wishPower = player.Creature.GetPower<WishPower>();
            await PowerCmd.Remove(wishPower);

            MainFile.Logger.Info($"Wish: added {card.Id} to {player.NetId}'s hand.");
        }
        catch (Exception e)
        {
            MainFile.Logger.Error($"Wish compendium pick failed: {e}");
        }
    }
}