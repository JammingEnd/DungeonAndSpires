using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

public interface IOnMetamagicChanged
{
    Task OnMetamagicChanged(PlayerChoiceContext choiceContext, Player player, int current, int currentAfterChange);
}