using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public static class DipField
{
    public static readonly SpireField<PlayerCombatState, DipState> State = new(() => null);

    // The visual DipCardInfo control attached to a card's currently-active NCard view.
    public static readonly SpireField<CardModel, Control> UI = new(() => null);
}