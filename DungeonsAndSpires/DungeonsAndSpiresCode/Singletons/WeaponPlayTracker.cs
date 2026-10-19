using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

public class WeaponPlayTracker() : CustomSingletonModel(HookType.Combat)
{
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        var state = player.PlayerCombatState?.GetWeaponPlay();
        if (state != null)
        {
            state.WeaponAttacks = 0;
            state.TwoHanded = 0;
            state.LastWasTwoHandedAttack = false;
            state.EnergySpent = 0;
        }
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        var state = cardPlay.Player.PlayerCombatState?.GetWeaponPlay();
        if (state == null)
        {
            return Task.CompletedTask;
        }

        bool twoHandedAttack = card.Type == CardType.Attack
            && card is MartialWeaponCard martial && martial.TwoHandedPower != null;
        state.LastWasTwoHandedAttack = twoHandedAttack;

        if (!card.Tags.Contains(DASCoreCardtags.Weapon))
        {
            return Task.CompletedTask;
        }

        if (card.Type == CardType.Attack)
        {
            state.WeaponAttacks += 1;
        }
        if (twoHandedAttack)
        {
            state.TwoHanded += 1;
        }
        return Task.CompletedTask;
    }
}
