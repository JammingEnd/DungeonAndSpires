using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;

[Pool(typeof(BarbarianRelicPool))]
public class BeserkerRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<HopiumRelic>();
    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    private bool _retaliatedThisTurn;

    // act1: at the start of your turn, while raged, a random 1 cost attack in your hand is free to play
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }

        _retaliatedThisTurn = false;

        if (CurrentLevel >= 1 && Owner.Creature.GetPower<BarbarianRagePower>() != null)
        {
            var oneCosts = Owner.PlayerCombatState?.Hand.Cards
                .Where(c => c.Type == CardType.Attack && c.EnergyCost.GetResolved() == 1).ToList();

            if (oneCosts is { Count: > 0 })
            {
                var card = Owner.RunState.Rng.CombatCardSelection.NextItem(oneCosts);
                card.EnergyCost.SetThisTurnOrUntilPlayed(0);
                card.InvokeEnergyCostChanged();
            }
        }
    }

    // act2: while raged, weak is 50% less effective
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (CurrentLevel < 2 || dealer != Owner.Creature || target == dealer)
        {
            return 1m;
        }
        if (Owner.GetRelic<HopiumRelic>() != null)
        {
            return 1m;
        }

        if (Owner.Creature.GetPower<BarbarianRagePower>() == null || dealer.GetPowerAmount<WeakPower>() <= 0 || !props.IsPoweredAttack())
        {
            return 1m;
        }

        // weak deals 25% less damage, so 12.5%, aka 7/6
        return 7m / 6m;
    }

    // act3: once per turn, when you take damage from an enemy attack, play a random attack card in your discard pile
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (CurrentLevel < 3 || _retaliatedThisTurn || target != Owner.Creature || dealer == null || dealer.IsPlayer || result.UnblockedDamage <= 0)
        {
            return;
        }

        var attacks = PileType.Discard.GetPile(Owner).Cards.Where(c => c.Type == CardType.Attack).ToList();
        if (attacks.Count == 0)
        {
            return;
        }

        _retaliatedThisTurn = true;
        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(attacks);
        await CardCmd.AutoPlay(choiceContext, card, null);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BarbarianRagePower>(),
    ];
}
