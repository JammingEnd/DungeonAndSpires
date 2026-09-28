using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortbowRopeArrowPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;
    private int _dexAmount;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([new IntVar("Dexterity", 0)]);

    public void SetDexterity(int amount)
    {
        _dexAmount = amount;
        DynamicVars["Dexterity"].BaseValue = amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        await PowerCmd.Apply<ShortbowRopeArrowDexterityPower>(choiceContext, Owner, _dexAmount, Owner, null);
        await PlayerCmd.GainEnergy(1, player);

        decimal newAmount = await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
        if (newAmount <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}