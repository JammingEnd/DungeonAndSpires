using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class MaulEarthshakerPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this || power.Owner != Owner || amount <= 0)
        {
            return;
        }

        var enemies = Owner.CombatState?.HittableEnemies;
        if (enemies == null)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, enemies, Amount, ValueProp.Unpowered, Owner, null, null);
    }
}
