using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;

[Pool(typeof(FighterRelicPool))]
public class BattleMasterRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;

    //at the start of combat, select a Maneuver.
    public override async Task InitialBonus() { }

    // at the end of your turn, select a card to retain for a turn.
    public override async Task Act2Bonus() { }

    //  after playing action Surge, gain a Maneuver.
    public override async Task Act3Bonus() { }

    private CardModel? _retainedByRelic;

    public override async Task BeforeCombatStart()
    {
        if (CurrentLevel >= 1)
        {
            await OfferManeuver(new BlockingPlayerChoiceContext());
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }

        if (_retainedByRelic != null)
        {
            _retainedByRelic.RemoveKeyword(CardKeyword.Retain);
            _retainedByRelic = null;
        }
    }

    public override async Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player || CurrentLevel < 2 || _retainedByRelic != null)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(new LocString("relics", "DUNGEONSANDSPIRES-BATTLE_MASTER_RELIC.selectionPrompt"), 0, 1);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);
        var card = selected.FirstOrDefault();
        if (card == null)
        {
            return;
        }

        card.AddKeyword(CardKeyword.Retain);
        _retainedByRelic = card;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (CurrentLevel < 3 || cardPlay.Card.Owner != Owner || cardPlay.Card is not ActionSurge)
        {
            return;
        }

        await AddManeuver(ModelDb.Card<BaitAndSwitch>());
    }

    private async Task OfferManeuver(PlayerChoiceContext choiceContext)
    {
        var options = new List<CardModel>
        {
            Owner.RunState.CreateCard(ModelDb.Card<BaitAndSwitch>(), Owner),
            Owner.RunState.CreateCard(ModelDb.Card<QuickToss>(), Owner),
            Owner.RunState.CreateCard(ModelDb.Card<MenacingAttack>(), Owner)
        };

        var prefs = new CardSelectorPrefs(new LocString("relics", "DUNGEONSANDSPIRES-BATTLE_MASTER_RELIC.selectionPrompt"), 1, 1);
        var picked = await CardSelectCmd.FromSimpleGrid(choiceContext, options, Owner, prefs);
        var chosen = picked.FirstOrDefault();
        if (chosen == null)
        {
            return;
        }
        
        await AddManeuver(ModelDb.GetById<CardModel>(chosen.Id));
    }

    private async Task AddManeuver(CardModel canonical)
    {
        var combatState = Owner.Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var card = combatState.CreateCard(canonical, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}
