using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class WallOfForcePower : DungeonsAndSpiresPower, IOnPotencyChanged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private const int Step = 1;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([new IntVar("Block", 0)]);

    private class Data
    {
        public decimal BlockBase;
        public decimal BlockMult;
    }

    protected override object InitInternalData() => new Data();

    public void SetAmount(decimal blockBase, decimal blockMult)
    {
        var data = GetInternalData<Data>();
        data.BlockBase = blockBase;
        data.BlockMult = blockMult;
        Recompute();
    }

    public async Task OnPotencyChanged(PlayerChoiceContext choiceContext, Player player, int current, int currentAfterChange)
    {
        if (player != Owner.Player)
        {
            return;
        }
        Recompute();
    }

    private void Recompute()
    {
        int potency = Owner.Player?.PlayerCombatState?.GetPotency() ?? 0;
        var data = GetInternalData<Data>();
        DynamicVars["Block"].BaseValue = data.BlockBase + data.BlockMult * Math.Floor(potency / (decimal)Step);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner, DynamicVars["Block"].IntValue, ValueProp.Unpowered, null);

        await PowerCmd.Decrement(this);
    }
}