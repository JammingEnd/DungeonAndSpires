using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;

/// <summary>
/// A DynamicVar whose value is the player's current Ability Potency, read live from the
/// combat state. Outside of combat (or with no potency state) it resolves to 0.
/// </summary>
public class PotencyVar : DynamicVar
{
    public PotencyVar() : base("Potency", 0)
    {
    }

    public static decimal GetCurrentPotency(CardModel? card)
    {
        if (card == null)
            return 0;
        if (!CombatManager.Instance.IsInProgress || card.CombatState == null)
            return 0;
        return card.Owner.PlayerCombatState?.GetPotency() ?? 0;
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        PreviewValue = GetCurrentPotency(card);
    }

    protected override decimal GetBaseValueForIConvertible()
    {
        return _owner is CardModel card ? GetCurrentPotency(card) : BaseValue;
    }
}