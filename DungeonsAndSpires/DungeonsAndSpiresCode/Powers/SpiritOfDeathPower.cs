using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
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

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target == Owner)
        {
            GetInternalData<Data>().LostHp = true;
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        var data = GetInternalData<Data>();
        if (!data.LostHp && data.Cards.Count > 0)
        {
            foreach (var card in data.Cards)
            {
                await CardPileCmd.Add(card, PileType.Hand);
            }
            await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner, data.Cards.Count, Owner, null);
        }

        await PowerCmd.Remove(this);
    }
}