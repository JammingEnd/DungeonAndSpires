using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

[Pool(typeof(DASSpellCardPool))]
public abstract class SpellCard(int Level, int cost, CardType type, CardRarity rarity, TargetType target) : CoreCard(cost, type, rarity, target)
{
    public int Level { get; } = Level;

    /// <summary>
    /// Whether playing this spell consumes a spellslot (and gets the -1 energy discount when one
    /// is available). False for spells that shouldn't interact with spellslots (cards like Shield, since only the powers itself consumed spellslots).
    /// </summary>
    public virtual bool UsesSpellSlot => true;

    // Spells with a spell level of 0 are cantrips.
    protected override HashSet<CardTag> CanonicalTags => Level == 0 ? [DASCoreCardtags.Spell, DASCoreCardtags.Cantrip] : [DASCoreCardtags.Spell];

    // Spells cost 1 less energy while a spellslot of their level is available.
    public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, out decimal modifiedCost)
    {
        modifiedCost = originalCost;
        if (card == this && Level > 0 && UsesSpellSlot && Owner.PlayerCombatState?.HasAvailableSlotForLevel(Level) == true)
        {
            modifiedCost = originalCost - 1;
            return true;
        }
        return false;
    }

    // Consume the spellslot when a leveled spell is cast.
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this && Level > 0 && UsesSpellSlot && Owner.PlayerCombatState?.HasAvailableSlotForLevel(Level) == true)
        {
            await SpellslotsCmd.ConsumeSpellSlotForLevel(choiceContext, Owner, Level);
        }
        else
        {
            // Generate Arcane exhaustion of the spell's level
        }
    }
}