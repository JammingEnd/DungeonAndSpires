using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;

[Pool(typeof(RogueRelicPool))]
public class ArcaneTricksterRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    
    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    private const int HiddenPotency = 5;
    private bool _potencyGranted;

    // act1: choose spells
    public override async Task BeforeCombatStart()
    {
        _potencyGranted = false;

        if (CurrentLevel < 1)
        {
            return;
        }

        var options = ModelDb.CardPool<DASSpellCardPool>().AllCards
            .Where(c => c.Rarity is CardRarity.Common or CardRarity.Uncommon && c is SpellCard spell && (spell.Level == 0 || spell.Level == 1))
            .ToList();
        Owner.RunState.Rng.CombatCardGeneration.Shuffle(options);
        var spells = options.Take(5).ToList();
        var prefs = new CardSelectorPrefs(new LocString("relics", "DUNGEONSANDSPIRES-ARCANE_TRICKSTER_RELIC.selectionPrompt"), 2, 2);
        var picked = await CardSelectCmd.FromSimpleGrid(new BlockingPlayerChoiceContext(), spells, Owner, prefs);

        foreach (var chosen in picked)
        {
            var combatState = Owner.Creature.CombatState;
            if (combatState == null)
            {
                continue;
            }

            var card = combatState.CreateCard(ModelDb.GetById<CardModel>(chosen.Id), Owner);
            card.AddKeyword(CardKeyword.Retain);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
        }
    }

    // act2: while hidden, gain 5 potency
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount,
        Creature? applier, CardModel? cardSource)
    {
        if (CurrentLevel < 2 || _potencyGranted || amount <= 0 || power is not RogueHiddenPower || power.Owner != Owner.Creature)
        {
            return;
        }

        _potencyGranted = true;
        await PotencyCmd.Add(choiceContext, Owner, HiddenPotency);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result,
        ValueProp props, Creature? dealer, CardModel? cardSource)
    {
        if (target == Owner.Creature)
        {
            await SyncHiddenPotency(choiceContext);
        }
    }


    private async Task SyncHiddenPotency(PlayerChoiceContext choiceContext)
    {
        if (!_potencyGranted || Owner.Creature.GetPower<RogueHiddenPower>() != null)
        {
            return;
        }

        _potencyGranted = false;
        await PotencyCmd.Remove(choiceContext, Owner, HiddenPotency);
    }

    // act3: enemies lose temp strength after playing a spell
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await SyncHiddenPotency(choiceContext);

        if (CurrentLevel < 3 || cardPlay.Card.Owner != Owner || cardPlay.Card is not SpellCard)
        {
            return;
        }

        var enemies = Owner.Creature.CombatState?.HittableEnemies.ToList() ?? [];
        foreach (var enemy in enemies)
        {
            await PowerCmd.Apply<StrengthSapPower>(choiceContext, enemy, 3, Owner.Creature, cardPlay.Card);
        }
    }
}
