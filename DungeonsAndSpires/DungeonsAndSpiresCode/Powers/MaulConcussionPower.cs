using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class MaulConcussionPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _vulnerable = 2;
    public void SetVulnerable(int amount) => _vulnerable = amount;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (cardPlay.Player != Owner.Player || card.Type != CardType.Attack || !card.Keywords.Contains(CoreKeywords.Bludgeoning))
        {
            return;
        }

        bool last = Amount <= 1;
        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, 1, Owner, card);
            if (last)
            {
                await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, _vulnerable, Owner, card);
            }
        }

        if (last)
        {
            await PowerCmd.Remove(this);
        }
        else
        {
            await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
        }
    }
}
