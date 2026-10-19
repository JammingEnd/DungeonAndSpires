using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class FighterSecondWindPower : DungeonsAndSpiresPower
{
    public const int AttacksRequired = 6;

    public override PowerType Type => PowerType.Buff;
    
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _attacks;
    
    public override int DisplayAmount => AttacksRequired - _attacks;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player || cardPlay.Card.Type != CardType.Attack)
        {
            return;
        }

        _attacks++;
        if (_attacks >= AttacksRequired)
        {
            _attacks = 0;
            InvokeDisplayAmountChanged();
            await PowerCmd.Apply<SecondWindPower>(choiceContext, Owner, 1, Owner, null);
        }

        InvokeDisplayAmountChanged();
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SecondWindPower>()
    ];
}
