using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;

[Pool(typeof(FighterRelicPool))]
public class EldritchKnightRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    
    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    // at the start of combat, select 3 of 9 common/uncommon spells to add to your hand.
    public override async Task BeforeCombatStart()
    {
        if (CurrentLevel < 1)
        {
            return;
        }

        var allSpells = ModelDb.CardPool<DASSpellCardPool>().AllCards
            .Where(c => c.Rarity is CardRarity.Common or CardRarity.Uncommon)
            .ToList();
        Owner.RunState.Rng.CombatCardGeneration.Shuffle(allSpells);
        var spells = allSpells.Take(9).ToList();
        if (spells.Count == 0)
        {
            return;
        }

        var options = spells.Select(c => Owner.RunState.CreateCard(c, Owner)).ToList();
        var prefs = new CardSelectorPrefs(new LocString("relics", "DUNGEONSANDSPIRES-ELDRITCH_KNIGHT_RELIC.selectionPrompt"), 3, Math.Min(3, options.Count));
        var context = new BlockingPlayerChoiceContext();
        var picked = await CardSelectCmd.FromSimpleGrid(context, options, Owner, prefs);

        foreach (var token in picked)
        {
            var combatState = Owner.Creature.CombatState;
            if (combatState == null)
            {
                return;
            }

            var card = combatState.CreateCard(ModelDb.GetById<CardModel>(token.Id), Owner);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Draw, Owner, CardPilePosition.Random);
        }
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
    
}
