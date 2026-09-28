using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class AstralGatePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private readonly List<CardModel> _marked = new();

    public bool IsMarked(CardModel card) => _marked.Contains(card);

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        _marked.Clear();
        var hand = PileType.Hand.GetPile(player).Cards.Where(c => c is SpellCard).ToList();
        for (int i = 0; i < Amount && hand.Count > 0; i++)
        {
            var card = player.RunState.Rng.CombatCardSelection.NextItem(hand);
            _marked.Add(card);
            hand.Remove(card);
        }

        var guh = this.DynamicVars.Values.Where(x => x.GetType().GetField("_amount") != null);
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (!IsMarked(card))
        {
            return false;
        }
        modifiedCost = 0;
        return true;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!IsMarked(cardPlay.Card))
        {
            return;
        }
        if (cardPlay.Card is SpellCard spell)
        {
            spell._shouldConsumeSpellslot = false;
        }
        _marked.Remove(cardPlay.Card);
    }
}