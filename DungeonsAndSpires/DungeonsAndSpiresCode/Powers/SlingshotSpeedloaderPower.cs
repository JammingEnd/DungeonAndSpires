using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// Draw 2 cards per amount at the start of your turn
// you can play them but whenever you play half of the cards drawn, the rest is discarded
public class SlingshotSpeedloaderPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private readonly List<CardModel> _batch = new();
    private int _playsLeft;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

    public void UpdateDrawAmount() => DynamicVars.Cards.BaseValue = Amount * 2;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        _batch.Clear();
        // Draw returns drawn cards, so i dont need to manually track them in a pathc or singleton :)
        _batch.AddRange(await CardPileCmd.Draw(choiceContext, 2 * Amount, player));
        _playsLeft = Amount;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_batch.Count == 0 || !_batch.Contains(cardPlay.Card))
        {
            return;
        }

        _playsLeft--;
        if (_playsLeft > 0)
        {
            return;
        }

        foreach (var other in _batch.Where(c => c != cardPlay.Card && c.Pile?.Type == PileType.Hand).ToList())
        {
            await CardCmd.Discard(choiceContext, other);
        }

        _batch.Clear();
    }
}
