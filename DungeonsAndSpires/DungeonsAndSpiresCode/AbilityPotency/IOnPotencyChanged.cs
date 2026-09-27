using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

public interface IOnPotencyChanged
{
    Task OnPotencyChanged(PlayerChoiceContext choiceContext, Player player, int current, int currentAfterChange);
}