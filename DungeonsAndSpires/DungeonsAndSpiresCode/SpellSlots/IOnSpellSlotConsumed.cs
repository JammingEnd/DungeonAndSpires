using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;

public interface IOnSpellSlotConsumed
{
    Task OnSpellSlotConsumed(PlayerChoiceContext choiceContext, Player player, int level, int amount);
}