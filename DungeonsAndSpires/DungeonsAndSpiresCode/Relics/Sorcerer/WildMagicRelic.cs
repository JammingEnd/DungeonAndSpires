using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Sorcerer;

[Pool(typeof(SorcererRelicPool))]
public class WildMagicRelic : SubclassRelic, IOnSpellSlotAmountChanged
{
    public override RelicRarity Rarity => RelicRarity.None;
    public override async Task InitialBonus()
    {
        // spellslot thingy
    }

    // potency equal to status cards
    public override async Task Act2Bonus()
    {
        Act2Active = true;
    }

    private bool _act2Active = false;

    [SavedProperty]
    public bool Act2Active
    {
        get => _act2Active;
        set => _act2Active = value;
    }

    private int previous;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner && Act2Active)
            return;
        
        int statuses = Owner.PlayerCombatState.AllCards.Count(x => x.Type == CardType.Status && x.Pile.Type != PileType.Exhaust);
        await PotencyCmd.SetTemp(choiceContext, Owner, statuses);
        previous = statuses;
    }

    public override async Task Act3Bonus()
    {
        Act3Active = true;
    }
    private bool _act3Active = false;

    [SavedProperty]
    public bool Act3Active
    {
        get => _act3Active;
        set => _act3Active = value;
    }

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != Owner && Act3Active)
            return;
        
        if(card.Type != CardType.Status)
            return;
        
        await PlayerCmd.GainEnergy(1, Owner);
    }

    public async Task OnSpellSlotAmountChanged(PlayerChoiceContext choiceContext, Player player, int level, int oldAmount,
        int currentAmount)
    {
        if (oldAmount > 0 && currentAmount == 0)
        {
            var pile = player.PlayerCombatState.DrawPile.Cards;
            var spellCards = pile.Where(c => c is SpellCard).ToArray();
            var selected = player.Creature.CombatState.RunState.Rng.CombatCardSelection.NextItem(spellCards);
            if (selected != null)
            {
                await CardPileCmd.Add(selected, PileType.Hand);
            }
        }
    }
}