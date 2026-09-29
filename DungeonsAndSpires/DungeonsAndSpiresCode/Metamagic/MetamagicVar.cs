using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

/// <summary>
/// A DynamicVar whose value is the player's current Metamagic, read live from the
/// combat state. Outside of combat (or with no metamagic state) it resolves to 0.
/// </summary>
public class MetamagicVar : DynamicVar
{
    public MetamagicVar() : base("Metamagic", 0)
    {
    }

    public static decimal GetCurrentMetamagic(CardModel? card)
    {
        if (card == null)
            return 0;
        if (!CombatManager.Instance.IsInProgress || card.CombatState == null)
            return 0;
        return card.Owner.PlayerCombatState?.GetMetamagic() ?? 0;
    }

    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target, bool runGlobalHooks)
    {
        PreviewValue = GetCurrentMetamagic(card);
    }

    protected override decimal GetBaseValueForIConvertible()
    {
        return _owner is CardModel card ? GetCurrentMetamagic(card) : BaseValue;
    }
}