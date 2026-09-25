using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

public interface IOnPotencyGained
{
    Task OnPotencyGained(PlayerChoiceContext choiceContext, Player player, int amount);
}