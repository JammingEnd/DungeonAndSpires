using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class WallOfIcePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        int frostbite = CombatState.HittableEnemies.Sum(e => e.GetPowerAmount<FrostbitePower>());
        if (frostbite <= 0)
        {
            return;
        }

        await PowerCmd.Apply<WallOfIceDexterityPower>(choiceContext, Owner, frostbite, Owner, null);
    }
}