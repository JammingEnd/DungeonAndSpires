using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;

[Pool(typeof(RogueRelicPool))]
public class AssassinRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    
    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    private bool _firstAttackUsed;
    
    [SavedProperty]
    public int BankedVigor { get; set; }

    public override Task BeforeCombatStart()
    {
        _firstAttackUsed = false;
        return Task.CompletedTask;
    }

    // act1: the first attack deals double damage
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (CurrentLevel < 1 || _firstAttackUsed || dealer != Owner.Creature || cardSource == null || target == dealer)
        {
            return 1m;
        }

        return cardSource.Owner == Owner && cardSource.Type == CardType.Attack ? 2m : 1m;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CurrentLevel >= 1 && !_firstAttackUsed && cardPlay.Card.Owner == Owner && cardPlay.Card.Type == CardType.Attack)
        {
            _firstAttackUsed = true;
        }

        return Task.CompletedTask;
    }

    // act2: entering a ? stores vigor
    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (CurrentLevel >= 2 && room.RoomType == RoomType.Event)
        {
            BankedVigor += 4;
        }

        return Task.CompletedTask;
    }

    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        await base.BeforeSideTurnStart(choiceContext, side, participants, combatState);

        if (side != CombatSide.Player || !participants.Contains(Owner.Creature) || Owner.PlayerCombatState.TurnNumber != 1)
        {
            return;
        }

        if (BankedVigor <= 0)
        {
            return;
        }

        int vigor = BankedVigor;
        BankedVigor = 0;
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, vigor, Owner.Creature, null);
    }

    // act3: entering hide gives str en dex
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (CurrentLevel < 3 || amount <= 0 || power is not RogueHiddenPower || power.Owner != Owner.Creature)
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, 1, Owner.Creature, null);
    }
}
