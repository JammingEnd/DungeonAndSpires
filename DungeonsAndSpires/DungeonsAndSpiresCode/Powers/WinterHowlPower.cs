using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class WinterHowlPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (amount <= 0 || power is not FrostbitePower frostbite || frostbite.Amount < 10)
        {
            return;
        }

        frostbite.SetAmount(1);

        if (Owner.CombatState == null)
        {
            return;
        }

        foreach (var player in Owner.CombatState.Players)
        {
            await CreatureCmd.GainBlock(player.Creature, 15, ValueProp.Move, null);
        }
    }
}
