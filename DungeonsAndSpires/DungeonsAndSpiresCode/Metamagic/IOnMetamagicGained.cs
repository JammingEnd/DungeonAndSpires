using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

public interface IOnMetamagicGained
{
    Task OnMetamagicGained(PlayerChoiceContext choiceContext, Player player, int amount);
}