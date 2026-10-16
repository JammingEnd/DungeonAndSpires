using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

// Per-turn counts of weapon attacks / two-handed weapons played, for Halberd payoffs.
public class WeaponPlayTracker() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState?.GetWeaponPlay();
        if (state != null)
        {
            state.WeaponAttacks = 0;
            state.TwoHanded = 0;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (!card.Tags.Contains(DASCoreCardtags.Weapon))
        {
            return Task.CompletedTask;
        }

        var state = cardPlay.Player.PlayerCombatState?.GetWeaponPlay();
        if (state == null)
        {
            return Task.CompletedTask;
        }

        if (card.Type == CardType.Attack)
        {
            state.WeaponAttacks += 1;
        }
        if (card is MartialWeaponCard martial && martial.TwoHandedPower != null)
        {
            state.TwoHanded += 1;
        }
        return Task.CompletedTask;
    }
}
