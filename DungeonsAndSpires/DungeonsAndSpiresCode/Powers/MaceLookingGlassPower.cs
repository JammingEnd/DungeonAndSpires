using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class MaceLookingGlassPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _discarded;

    public override int DisplayAmount => _discarded;

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card.Owner != Owner.Player)
        {
            return;
        }

        _discarded++;
        InvokeDisplayAmountChanged();

        if (_discarded < 3)
        {
            return;
        }

        _discarded -= 3;
        InvokeDisplayAmountChanged();
        await PowerCmd.Apply<ResonancePower>(choiceContext, Owner, Amount, Owner, null);
    }
}
