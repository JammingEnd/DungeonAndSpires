using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

[Pool(typeof(DASSimpleWeaponCardPool))]
public abstract class SimpleWeaponCard(int cost, CardType type, CardRarity rarity, TargetType target) : CoreCard(cost, type, rarity, target)
{
    protected override HashSet<CardTag> CanonicalTags => [DASCoreCardtags.Weapon];

    // Weapons imbued with Potent (e.g. by Catapult) add their owner's Ability Potency to the damage
    // they deal, until they have been played once.
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource == this && dealer == Owner.Creature && Keywords.Contains(CoreKeywords.Potent))
        {
            return Owner.PlayerCombatState?.GetPotency() ?? 0;
        }
        return 0;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this && Keywords.Contains(CoreKeywords.Potent))
        {
            RemoveKeyword(CoreKeywords.Potent);
        }
    }
}