using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class Reverberation : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    private const int _threshold = 4;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target,
        DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner || cardSource == null)
        {
            return;
        }

        if (cardSource.Keywords.Contains(CoreKeywords.Thunder))
        {
            await PowerCmd.ModifyAmount(choiceContext, this, 1, null, cardSource);
        }
        Trigger(choiceContext);
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if(power != this)
            return;
        Trigger(choiceContext);
    }

    async void Trigger(PlayerChoiceContext ctx)
    {
        if (Amount >= _threshold)
        {
            await PowerCmd.Apply<WeakPower>(ctx, Owner, 2, Applier, null);
            await PlayerCmd.GainEnergy(1, Applier.Player);
            await PowerCmd.ModifyAmount(ctx, this, -Amount, null, null);
        }
    }

}