using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

public class WeaponDipsHandler : CustomSingletonModel
{
    public WeaponDipsHandler() : base(HookType.Combat)
    {
        // Dips are a per-combat effect; clear any leftovers from a previous combat.
        CombatManager.Instance.CombatSetUp += OnCombatSetUp;
    }

    private void OnCombatSetUp(CombatState state)
    {
        foreach (var player in state.Players)
        {
            player.PlayerCombatState?.GetDipState()?.WeaponDips.Clear();
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DipCmd.Use(choiceContext, cardPlay.Card.Owner, cardPlay.Card);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (dealer == null || !dealer.IsPlayer)
            return 0;
        var state = dealer.Player.PlayerCombatState?.GetDipState();
        if (state == null)
            return 0;
        if (!state.WeaponDips.ContainsKey(cardSource))
            return 0;
        return state.WeaponDips[cardSource].Strength;
    }

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        var player = card.Owner;
        var state = player.PlayerCombatState?.GetDipState();
        if (state == null)
            return false;
        if (!state.WeaponDips.ContainsKey(card))
            return false;
        keywords.Add(state.WeaponDips[card].Type);
        return true;
    }
}