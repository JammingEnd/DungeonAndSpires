using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// At 4 stacks the next card pays out stats and energy, then the stacks are spent.
public class ResonancePower : DungeonsAndSpiresPower
{
    public const int Threshold = 4;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    public override async Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player || Amount < Threshold)
        {
            return;
        }

        var context = new ThrowingPlayerChoiceContext();

        switch (cardPlay.Card.Type)
        {
            case CardType.Attack:
                await PowerCmd.Apply<StrengthPower>(context, Owner, 1, Owner, null);
                break;
            case CardType.Skill:
                await PowerCmd.Apply<DexterityPower>(context, Owner, 1, Owner, null);
                break;
            case CardType.Power:
                await PowerCmd.Apply<StrengthPower>(context, Owner, 1, Owner, null);
                await PowerCmd.Apply<DexterityPower>(context, Owner, 1, Owner, null);
                break;
            default:
                return;
        }

        await PlayerCmd.GainEnergy(1, Owner.Player);
        await PowerCmd.ModifyAmount(context, this, -Threshold, null, null);
    }
}
