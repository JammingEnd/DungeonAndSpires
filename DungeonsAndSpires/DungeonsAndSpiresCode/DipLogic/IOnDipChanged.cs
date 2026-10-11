using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public interface IOnDipChanged
{
    Task OnDipChanged(PlayerChoiceContext choiceContext, Player player, CardModel card, DipInfo dipInfo);
}