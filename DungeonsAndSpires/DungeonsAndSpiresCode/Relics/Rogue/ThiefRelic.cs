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
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;

[Pool(typeof(RogueRelicPool))]
public class ThiefRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;

    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    private bool _endedHidden;
    private bool _eliteBonusPending;

    public override Task BeforeCombatStart()
    {
        _endedHidden = false;
        _eliteBonusPending = CurrentLevel >= 3 && Owner.RunState.CurrentRoom is { RoomType: RoomType.Elite or RoomType.Boss };
        return Task.CompletedTask;
    }

    // act1: damage taken outside of combat is halved
    public override decimal ModifyHpLostBeforeOsty(Creature target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (CurrentLevel < 1 || target != Owner.Creature || CombatManager.Instance.IsInProgress)
        {
            return amount;
        }

        return Math.Ceiling(amount / 2m);
    }

    // act2: end combat while hidden gives more gold
    public override Task AfterCombatVictory(CombatRoom room)
    {
        _endedHidden = CurrentLevel >= 2 && Owner.Creature.GetPower<RogueHiddenPower>() != null;
        return Task.CompletedTask;
    }

    public override Task BeforeCombatRewardOffered(RewardsSet rewards, CombatRoom room)
    {
        if (!_endedHidden || rewards.Player != Owner)
        {
            return Task.CompletedTask;
        }

        for (int i = 0; i < rewards.Rewards.Count; i++)
        {
            if (rewards.Rewards[i] is GoldReward gold)
            {
                rewards.Rewards[i] = new GoldReward((int)Math.Ceiling(gold.Amount * 1.25m), Owner);
            }
        }

        _endedHidden = false;
        return Task.CompletedTask;
    }

    // act3: start boss/elite combats with energy and block
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || !_eliteBonusPending || Owner.PlayerCombatState.TurnNumber != 1)
        {
            return;
        }

        _eliteBonusPending = false;
        await PlayerCmd.GainEnergy(2, Owner);
        await CreatureCmd.GainBlock(Owner.Creature, 20, ValueProp.Unpowered, null);
    }
}
