using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class FindFamiliarPower : DungeonsAndSpiresPower, IOnSpellSlotConsumed
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private class Data
    {
        public int Threshold;
    }

    protected override object InitInternalData() => new Data();

    public void SetThreshold(int threshold)
    {
        GetInternalData<Data>().Threshold = threshold;
    }

    public async Task OnSpellSlotConsumed(PlayerChoiceContext choiceContext, Player player, int level, int amount)
    {
        if (player != Owner.Player)
        {
            return;
        }
        if (level < GetInternalData<Data>().Threshold)
        {
            return;
        }

        await CardPileCmd.Draw(choiceContext, Amount, player);
    }
}