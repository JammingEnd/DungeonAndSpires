using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class SpiritOfDeathPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    protected override bool IsVisibleInternal => false;

    private class Data
    {
        // for multiple spirit of deaths
        public List<CardModel> Cards { get; } = new();
        public bool LostHp;
    }

    protected override object InitInternalData() => new Data();

    public void SetCard(CardModel card)
    {
        GetInternalData<Data>().Cards.Add(card);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner && result.UnblockedDamage >= 1m)
        {
            GetInternalData<Data>().LostHp = true;
        }
    }

    // At the start of your turn, if you didn't lose HP, return the card(s) and gain 1 energy each.
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var data = GetInternalData<Data>();
        if (!data.LostHp)
        {
            int returned = 0;
            foreach (var card in data.Cards)
            {
                // Only return cards that were actually played (they sit in the discard pile).
                if (card.Pile?.Type != PileType.Discard)
                {
                    continue;
                }
                await CardPileCmd.Add(card, PileType.Hand);
                returned++;
            }

            if (returned > 0)
            {
                await PlayerCmd.GainEnergy(returned, Owner.Player);
            }
        }

        await PowerCmd.Remove(this);
    }
}