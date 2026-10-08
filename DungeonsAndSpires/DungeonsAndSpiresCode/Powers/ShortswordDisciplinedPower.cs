using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortswordDisciplinedPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
    

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.EnergyCost.Canonical >= 2 && card.Tags.Contains(DASCoreCardtags.Weapon))
        {
            var pile = Owner.Player.PlayerCombatState.DiscardPile.Cards;
            var cheaps = pile.Where(c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.EnergyCost.Canonical <= 1).ToArray();
            var selected = Owner.CombatState.RunState.Rng.CombatCardSelection.NextItem(cheaps);
            await CardPileCmd.Add(selected, PileType.Hand);
        }
    }
    
}