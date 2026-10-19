using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public static class DipEffects
{
    // Apply a card's Dip to a target immediately (used by Backbreaker / Loaded Bolt).
    public static async Task Apply(PlayerChoiceContext choiceContext, Player player, CardModel card, Creature target, CardModel source)
    {
        var dip = player.PlayerCombatState?.GetDip(card);
        if (dip == null)
        {
            return;
        }

        int amount = WeaponDipsHandler.EffectiveStrength(card, dip, player.Creature);
        if (amount <= 0)
        {
            return;
        }

        if (dip.Type == CoreKeywords.Poison)
        {
            await PowerCmd.Apply<PoisonPower>(choiceContext, target, amount, player.Creature, source);
        }
        else if (dip.Type == CoreKeywords.Cold)
        {
            await PowerCmd.Apply<FrostbitePower>(choiceContext, target, amount, player.Creature, source);
        }
        else if (dip.Type == CoreKeywords.Lightning)
        {
            await PowerCmd.Apply<ShockedPower>(choiceContext, target, amount, player.Creature, source);
        }
        else if (dip.Type == CoreKeywords.Thunder && target.GetPowerAmount<Reverberation>() == 0)
        {
            await PowerCmd.Apply<Reverberation>(choiceContext, target, 1, player.Creature, source);
        }
    }
}
