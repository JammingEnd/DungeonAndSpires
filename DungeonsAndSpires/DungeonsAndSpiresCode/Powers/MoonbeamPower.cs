using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class MoonbeamPower : DungeonsAndSpiresPower, IOnPotencyChanged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private const int Step = 2;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([
            new IntVar("Damage", 0)
        ]);

    private class Data
    {
        public decimal DamageBase;
        public decimal DamageMult;
    }

    protected override object InitInternalData() => new Data();

    public void SetAmounts(decimal damageBase, decimal damageMult)
    {
        var data = GetInternalData<Data>();
        data.DamageBase = damageBase;
        data.DamageMult = damageMult;
        Recalculate();
    }

    public async Task OnPotencyChanged(PlayerChoiceContext choiceContext, Player player, int current, int currentAfterChange)
    {
        if (player != Owner.Player)
        {
            return;
        }
        Recalculate();
    }

    private void Recalculate()
    {
        int potency = Owner.Player?.PlayerCombatState?.GetPotency() ?? 0;
        var data = GetInternalData<Data>();
        DynamicVars["Damage"].BaseValue = (data.DamageBase + data.DamageMult * Math.Floor(potency / (decimal)Step)) * Amount;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            Recalculate();
            return;
        }

        if (power.Owner?.Side != CombatSide.Enemy)
        {
            return;
        }
        // Only fires when a power is reduced to 0.
        if (amount >= 0 || power.Amount > 0)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, power.Owner, DynamicVars["Damage"].IntValue, ValueProp.Unpowered, Owner, null, null);
    }
}