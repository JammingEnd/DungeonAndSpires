using MegaCrit.Sts2.Core.Extensions;
using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Ancients;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.RestSite;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;

[Pool(typeof(RogueRelicPool))]
public class ArcaneTricksterRelic : SubclassRelic, IOnAncientDialogueDone, IOnRestSiteSpellOption
{
    public override RelicRarity Rarity => RelicRarity.None;


    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<GhostGlovesRelic>();

    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    private const int HiddenPotency = 5;
    private bool _potencyGranted;
    [SavedProperty]
    public List<SerializableCard> PickedSpells { get; set; } = new();
    // act1: choose spells
    public async Task OnAncientDialogueDone(AncientEventModel ancient)
    {
        if (CurrentLevel < 1)
        {
            return;
        }

        await OptionSelection();
    }
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player != Owner || CurrentLevel < 1)
        {
            return false;
        }

        options.Add(new SpellSelectionRestSiteOption(player));
        return true;
    }
    private async Task OptionSelection()
    {
        var picked = await GetPickedSpells();
        
        foreach (var stored in PickedSpells)
        {
            if (stored.Id is not { } id)
            {
                continue;
            }
            var card = Owner.Deck.Cards.FirstOrDefault(c => c.Id == id && c.CurrentUpgradeLevel == stored.CurrentUpgradeLevel);
            if (card != null)
            {
                await CardPileCmd.RemoveFromDeck(card);
            }
        }
        PickedSpells.Clear();

        foreach (var token in picked)
        {
            var card = Owner.RunState.CreateCard(token, Owner);
            card.AddKeyword(CardKeyword.Retain);
            var result = await CardPileCmd.Add(card, PileType.Deck);
            CardCmd.PreviewCardPileAdd(result); 
            
            PickedSpells.Add(result.cardAdded.ToSerializable());
        }
    }
    public async Task OnRestSiteSpellSelected(Player player)
    {
        await OptionSelection();
    }

    private async Task<IEnumerable<CardModel>> GetPickedSpells()
    {
        var options = ModelDb.CardPool<DASSpellCardPool>().AllCards
            .Where(c => c.Rarity is CardRarity.Common or CardRarity.Uncommon && c is SpellCard spell && (spell.Level <= 3 && spell.MultiplayerConstraint != CardMultiplayerConstraint.MultiplayerOnly))
            .ToList();
        options.UnstableShuffle(Owner.RunState.Rng.CombatCardGeneration);
        var spells = options.Take(5).ToList();
        var prefs = new CardSelectorPrefs(new LocString("relics", "DUNGEONSANDSPIRES-ARCANE_TRICKSTER_RELIC.selectionPrompt"), 2, 2);
        var context = new BlockingPlayerChoiceContext();
        return await CardSelectCmd.FromSimpleGrid(context, spells, Owner, prefs);
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
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<RogueHidePower>(),
    ];
}
