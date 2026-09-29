using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortswordBloodritePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        int count = PileType.Draw.GetPile(player).Cards.Count(c => c.Type == CardType.Curse || c.Type == CardType.Status);
        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, count * 2, Owner, null);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card.Owner != Owner.Player || (card.Type != CardType.Curse && card.Type != CardType.Status))
        {
            return;
        }

        await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -1, Owner, null);
    }
}