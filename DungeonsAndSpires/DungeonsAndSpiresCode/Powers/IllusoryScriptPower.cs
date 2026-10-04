using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class IllusoryScriptPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private class Data
    {
        public int MaxLevel;
    }

    protected override object InitInternalData() => new Data();

    private CardModel? _sourceCard;

    public void SetMaxLevel(int maxLevel)
    {
        GetInternalData<Data>().MaxLevel = maxLevel;
    }

    public void SetSourceCard(CardModel card)
    {
        _sourceCard = card;
    }

    private bool IsQualifying(CardModel card)
    {
        return card != _sourceCard && card is SpellCard spell && spell.Owner == Owner.Player && spell.Level >= 1 && spell.Level <= GetInternalData<Data>().MaxLevel;
    }

    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (IsQualifying(card))
        {
            modifiedCost = 0;
            return true;
        }
        return false;
    }

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (!IsQualifying(cardPlay.Card))
        {
            return;
        }

        cardPlay.Card.AddKeyword(CoreKeywords.Scrolled);
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (!IsQualifying(cardPlay.Card))
        {
            return;
        }

        // Powers' AfterCardPlayed runs before the card's, so clear the spellslot flag first.
        if (cardPlay.Card is SpellCard spell)
        {
            spell._shouldConsumeSpellslot = false;
        }

        await CardPileCmd.Add(cardPlay.Card, PileType.Exhaust);
        await PowerCmd.Remove(this);
    }
}