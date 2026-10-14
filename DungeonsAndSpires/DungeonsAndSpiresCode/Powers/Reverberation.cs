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
    
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
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
        if (Amount >= _threshold)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, Owner, 2, Applier, null);
            await PlayerCmd.GainEnergy(1, Applier.Player);
            await PowerCmd.ModifyAmount(choiceContext, this, -Amount, null, cardSource);
        }
    }
}