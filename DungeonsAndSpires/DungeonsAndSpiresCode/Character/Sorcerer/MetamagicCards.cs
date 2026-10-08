using BaseLib.Abstracts;
using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

[Pool(typeof(TokenCardPool))]
public abstract class MetamagicCards() : CustomCardModel(0, CardType.Skill, CardRarity.Token, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Ethereal,
        CardKeyword.Exhaust
    ];
}
//your next card that targets one enemy deal their effects to a random enemy too
public class QuickendSpell : MetamagicCards
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-QUICKEND_SPELL.selectionPrompt"), 1, 1);
        Func<CardModel, bool> filter = c => 
            c is SpellCard && 
            c.TargetType is not (TargetType.AllEnemies or TargetType.AllAllies) && 
            !c.Keywords.Contains(CoreKeywords.Quickened) && 
            c.Type != CardType.Power && 
            c.TargetType != TargetType.Self;
        if (Owner.Creature.CombatState.PlayerCreatures.Count > 1)
        {
            filter = c => 
                c is SpellCard && 
                c.TargetType is not (TargetType.AllEnemies or TargetType.AllAllies) && 
                !c.Keywords.Contains(CoreKeywords.Quickened) && 
                c.Type != CardType.Power;
        }
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, filter, this);
        var target = selected.FirstOrDefault();
        if (target != null)
        {
            target.AddKeyword(CoreKeywords.Quickened);
        }
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            if(this.Pile.Type == PileType.Exhaust)
                return;
            await CardPileCmd.Add(this, PileType.Exhaust);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Quickened)
    ];
}
//a card gains 1 level of potency (or 3 if potency 1) 
public class HeightenedSpell : MetamagicCards
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-HEIGHTENED_SPELL.selectionPrompt"), 1, 1);
        Func<CardModel, bool> filter = c => c is SpellCard spell && spell.PotencyVars.Count > 0;
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, filter, this);
        var target = selected.FirstOrDefault();
        if (target != null)
        {
            target.AddKeyword(CoreKeywords.Heightened);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Heightened)
    ];

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
        {
            if(this.Pile.Type == PileType.Exhaust)
                return;
            await CardPileCmd.Add(this, PileType.Exhaust);
        }
    }
}