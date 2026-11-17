using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityLogic;

/// <summary>
/// Implemented by a character model to expose a combat ability (e.g. the Barbarian's Rage) to the
/// ability button. The button fires <see cref="CastAbility"/> when pressed and shows
/// <see cref="AbilityTitle"/>/<see cref="AbilityDescription"/> on hover.
/// </summary>
public interface IAbilityCast
{
    /// Icon shown on the button, e.g. "res://DungeonsAndSpires/images/powers/barbarian_rage_power.png".
    string AbilityIconPath { get; }

    /// Number of charges to display on the label.
    int GetCharges(Player player);

    /// Maximum charges this ability starts each combat with.
    int MaxCharges { get; }

    /// Cast the ability.
    void CastAbility(Player player);

    /// Hover-tip title.
    LocString AbilityTitle { get; }

    /// Hover-tip body.
    LocString AbilityDescription { get; }
}
