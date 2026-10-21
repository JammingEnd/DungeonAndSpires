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
    public override PowerStackType StackType => PowerStackType.Counter;
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.EnergyCost.GetResolved() >= 2 && card.Type == CardType.Attack)
        {
            for (int i = 0; i < Amount; i++)
            {
                var pile = Owner.Player.PlayerCombatState.DiscardPile.Cards;
                var cheaps = pile.Where(c => (c.Tags.Contains(DASCoreCardtags.Weapon) || c.Tags.Contains(DASCoreCardtags.Cantrip))&& c.EnergyCost.GetResolved() < 1).ToArray();
                var selected = Owner.CombatState.RunState.Rng.CombatCardSelection.NextItem(cheaps);
                await CardPileCmd.Add(selected, PileType.Hand);
            }
        }
    }
    
}