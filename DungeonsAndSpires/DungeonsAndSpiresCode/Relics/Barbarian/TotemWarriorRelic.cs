using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;

[Pool(typeof(BarbarianRelicPool))]
public class TotemWarriorRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;

    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    // Combats left that start with the +2 energy from act2.
    [SavedProperty]
    public int BonusEnergyCombats { get; set; }

    // act1: whenever you enter rage, gain a random rage spirit (act3 doubles them)
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (CurrentLevel < 1 || amount <= 0 || power is not BarbarianRagePower || power.Owner != Owner.Creature)
        {
            return;
        }

        List<PowerModel> spirits =
        [
            ModelDb.Power<BearSpirit>(),
            ModelDb.Power<EagleSpirit>(),
            ModelDb.Power<WolfSpirit>(),
        ];
        if (Owner.Creature.CombatState.Allies.Count > 0)
        {
           spirits.Add(ModelDb.Power<ElkSpirit>());
        }
        var spirit = spirits[Owner.RunState.Rng.CombatCardGeneration.NextInt(spirits.Count)] as SpiritPower;
        int stacks = CurrentLevel >= 3 ? 2 : 1;
        spirit.SetLevel(stacks);
        await PowerCmd.Apply(choiceContext, spirit, Owner.Creature, 1, Owner.Creature, null);
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
}
