using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class WallOfFirePower : DungeonsAndSpiresPower, IOnPotencyChanged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private const int Step = 4;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        base.CanonicalVars.Concat([
            new IntVar("HitDamage", 0),
            new IntVar("EndDamage", 0)
        ]);

    private class Data
    {
        public decimal HitBase;
        public decimal HitMult;
        public decimal EndBase;
        public decimal EndMult;
        public decimal HeightenBonus;
    }

    protected override object InitInternalData() => new Data();

    public void SetAmounts(decimal hitBase, decimal hitMult, decimal endBase, decimal endMult, decimal heightenBonus = 0)
    {
        var data = GetInternalData<Data>();
        data.HitBase = hitBase;
        data.HitMult = hitMult;
        data.EndBase = endBase;
        data.EndMult = endMult;
        data.HeightenBonus = heightenBonus;
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
        decimal levels = Math.Floor(potency / (decimal)Step) + data.HeightenBonus;
        DynamicVars["HitDamage"].BaseValue = (data.HitBase + data.HitMult * levels) * Amount;
        DynamicVars["EndDamage"].BaseValue = (data.EndBase + data.EndMult * levels) * Amount;
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (power == this)
        {
            Recalculate();
        }
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target != Owner || dealer == null || dealer.Player != null)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, dealer, DynamicVars["HitDamage"].IntValue, ValueProp.Unpowered, Owner);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player)
        {
            return;
        }

        await CreatureCmd.Damage(choiceContext, CombatState.HittableEnemies, DynamicVars["EndDamage"].IntValue, ValueProp.Unpowered, Owner);
    }
}