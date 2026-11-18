using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;

[Pool(typeof(BarbarianRelicPool))]
public class TotemWarriorRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<FifthColumnRelic>();
    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }
    
    private bool _grantedSpirit = false;
    
    public void SetGrantedSpirit(bool grantedSpirit) => _grantedSpirit = grantedSpirit;

    // Combats left that start with the +2 energy from act2.
    [SavedProperty]
    public int BonusEnergyCombats { get; set; }

    // act1: whenever you enter rage, gain a random rage spirit (act3 doubles them)
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (CurrentLevel < 1 || power is not BarbarianRagePower || power.Owner != Owner.Creature)
        {
            return;
        }
        
        
        if(_grantedSpirit)
            return;

        List<PowerModel> spirits =
        [
            ModelDb.Power<BearSpirit>(),
            ModelDb.Power<EagleSpirit>(),
            ModelDb.Power<ElkSpirit>(),
        ];
        if (Owner.Creature.CombatState.Players.Count > 1)
        {
           spirits.Add(ModelDb.Power<WolfSpirit>());
        }
        var spirit = (SpiritPower)spirits[Owner.RunState.Rng.CombatCardGeneration.NextInt(spirits.Count)].ToMutable();
        int stacks = CurrentLevel >= 3 ? 2 : 1;
        spirit.SetLevel(stacks);
        await PowerCmd.Apply(choiceContext, spirit, Owner.Creature, 1, Owner.Creature, null);
        _grantedSpirit = true;
    }

    // act2: taking damage outside of combat starts the next 2 combats with 2 extra energy
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (CurrentLevel >= 2 && target == Owner.Creature && !CombatManager.Instance.IsInProgress)
        {
            BonusEnergyCombats = 2;
        }

        return amount;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || Owner.PlayerCombatState.TurnNumber != 1 || BonusEnergyCombats <= 0)
        {
            return;
        }

        BonusEnergyCombats--;
        await PlayerCmd.GainEnergy(2, Owner);
    }

    public override Task BeforeCombatStart()
    {
        _grantedSpirit = false;
        return base.BeforeCombatStart();
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BarbarianRagePower>(),
        HoverTipFactory.FromPower<BearSpirit>(),
        HoverTipFactory.FromPower<EagleSpirit>(),
        HoverTipFactory.FromPower<ElkSpirit>(),
        HoverTipFactory.FromPower<WolfSpirit>()
    ];
}
