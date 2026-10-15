using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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
        var card = cardPlay.Card;
        if (CheckState(card.Owner.Creature, card, out var info))
        {
            if (info.Type == CoreKeywords.Poison)
            {
                await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, info.Strength, card.Owner.Creature, card);
            }
        }
        await DipCmd.Use(choiceContext, cardPlay.Card.Owner, cardPlay.Card);
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (CheckState(dealer, cardSource, out var info))
        {
            if (info.Type == CoreKeywords.Poison)
                return 0;
            int homebrewAmount = dealer.GetPowerAmount<ScimitarHomebrewPower>();
            int multiplier = cardSource is CoreCard core ? core.DipStrengthMultiplier : 1;
            return info.Strength * multiplier + homebrewAmount;
        }
        return 0;
    }

    private bool CheckState(Creature? dealer, CardModel cardSource, out DipInfo dipInfo)
    {
        dipInfo = null;
        if (dealer == null || !dealer.IsPlayer || cardSource == null)
            return false;
        var state = dealer.Player.PlayerCombatState?.GetDipState();
        if (state == null)
            return false;
        if (!state.WeaponDips.ContainsKey(cardSource))
            return false;
        dipInfo = state.WeaponDips[cardSource];
        return true;
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