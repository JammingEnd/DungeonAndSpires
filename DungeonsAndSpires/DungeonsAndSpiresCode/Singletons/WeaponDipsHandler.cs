using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;
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
        // for clearing dips on cards at combat start
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
        if (CheckState(card.Owner.Creature, card, out var info) && info.Type == CoreKeywords.Poison)
        {
            // AoE weapon attacks have no single target.
            var targets = cardPlay.Target != null
                ? new List<Creature> { cardPlay.Target }
                : card.TargetType == TargetType.AllEnemies
                    ? card.Owner.Creature.CombatState?.HittableEnemies.ToList() ?? new List<Creature>()
                    : new List<Creature>();

            foreach (var target in targets)
            {
                await PowerCmd.Apply<PoisonPower>(choiceContext, target, EffectiveStrength(card, info, card.Owner.Creature), card.Owner.Creature, card);
            }
        }
        bool skipDipUse = false;
        if (cardPlay.Card is ScimitarQuickSlash quickSlash && !quickSlash.DipUsedThisTurn)
        {
            quickSlash.DipUsedThisTurn = true;
            skipDipUse = true;
        }
        if (!skipDipUse)
        {
            await DipCmd.Use(choiceContext, cardPlay.Card.Owner, cardPlay.Card);
        }
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (CheckState(dealer, cardSource, out var info) && info.Type != CoreKeywords.Poison)
        {
            return EffectiveStrength(cardSource!, info, dealer!);
        }
        return 0;
    }
    
    public static int EffectiveStrength(CardModel card, DipInfo info, Creature dealer)
    {
        int multiplier = card is CoreCard core ? core.DipStrengthMultiplier : 1;
        return info.Strength * multiplier + dealer.GetPowerAmount<ScimitarHomebrewPower>();
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