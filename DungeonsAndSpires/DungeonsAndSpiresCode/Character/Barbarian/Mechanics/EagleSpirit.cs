using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class EagleSpirit : SpiritPower
{
    private bool _drewThisTurn;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        _drewThisTurn = false;

        if (_level >= 2)
        {
            await CardPileCmd.Draw(choiceContext, 1, player);
        }
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (_level < 1 || _drewThisTurn || fromHandDraw || card.Owner != Owner.Player)
        {
            return;
        }

        _drewThisTurn = true;
        await CreatureCmd.GainBlock(Owner, 8, ValueProp.Unpowered, null);
    }
}
