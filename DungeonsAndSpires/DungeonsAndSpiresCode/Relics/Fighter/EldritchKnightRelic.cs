using MegaCrit.Sts2.Core.Extensions;
using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Ancients;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.RestSite;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;

[Pool(typeof(FighterRelicPool))]
public class EldritchKnightRelic : SubclassRelic, IOnAncientDialogueDone, IOnRestSiteSpellOption
{
    public override RelicRarity Rarity => RelicRarity.None;
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<ClearmetalBladeRelic>();
    
    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }
    
    [SavedProperty]
    public List<SerializableCard> PickedSpells { get; set; } = new();
    
    public async Task OnAncientDialogueDone(AncientEventModel ancient)
    {
        if (CurrentLevel < 1)
        {
            return;
        }

        if (ancient.Id == ModelDb.GetId<Neow>())
        {
           await OptionSelection();
        }
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

        foreach (var token in picked)
        {
            var result = await CardPileCmd.Add(token, PileType.Deck);
            CardCmd.PreviewCardPileAdd(result); 
            
            PickedSpells.Add(result.cardAdded.ToSerializable());
        }
    }
    public async Task OnRestSiteSpellSelected(Player player)
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
            var result = await CardPileCmd.Add(token, PileType.Deck);
            CardCmd.PreviewCardPileAdd(result); 
            
            PickedSpells.Add(result.cardAdded.ToSerializable());
        }
    }

    private async Task<IEnumerable<CardModel>> GetPickedSpells()
    {
        var allSpells = ModelDb.CardPool<DASSpellCardPool>().AllCards
            .Where(c => c.Rarity is CardRarity.Common or CardRarity.Uncommon)
            .ToList();
        allSpells.UnstableShuffle(Owner.RunState.Rng.CombatCardGeneration);

        bool noClearMetal = Owner.GetRelic<ClearmetalBladeRelic>() == null;
        int total = noClearMetal ? 9 : 18;
        
        var spells = allSpells.Take(total).ToList();
        if (spells.Count == 0)
        {
            return [];
        }

        var options = spells.Select(c => Owner.RunState.CreateCard(c, Owner)).ToList();
        CardSelectorPrefs prefs;
        if (noClearMetal)
        {
            prefs = new CardSelectorPrefs(new LocString("relics", "DUNGEONSANDSPIRES-ELDRITCH_KNIGHT_RELIC.selectionPrompt"), 3, Math.Min(3, options.Count));
        }
        else
        {
            prefs = new CardSelectorPrefs(new LocString("relics", "DUNGEONSANDSPIRES-ELDRITCH_KNIGHT_RELIC.selectionPrompt"), 0, Math.Min(6, options.Count));
        }
        var context = new BlockingPlayerChoiceContext();
        return await CardSelectCmd.FromSimpleGrid(context, options, Owner, prefs);
    }
    // drawing a spell grants 5 Vigor.
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (CurrentLevel < 2 || card.Owner != Owner || card is not SpellCard)
        {
            return;
        }

        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, 3, Owner.Creature, null);
    }

    // after playing a spell, a random enemy gains 1 vulnerable.
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CurrentLevel < 3 || cardPlay.Card.Owner != Owner || cardPlay.Card is not SpellCard)
        {
            return;
        }

        var enemies = Owner.Creature.CombatState.HittableEnemies;
        var selectedEnemy = Owner.RunState.Rng.CombatTargets.NextItem(enemies);
        await PowerCmd.Apply<VulnerablePower>(choiceContext, selectedEnemy, 1, Owner.Creature, null);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FighterSecondWindPower>(),
    ];
}
