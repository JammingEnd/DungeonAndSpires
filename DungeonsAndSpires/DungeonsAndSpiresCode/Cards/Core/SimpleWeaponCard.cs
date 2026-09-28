using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

[Pool(typeof(DASSimpleWeaponCardPool))]
public abstract class SimpleWeaponCard(int cost, CardType type, CardRarity rarity, TargetType target) : CoreCard(cost, type, rarity, target)
{
    protected override HashSet<CardTag> CanonicalTags => [DASCoreCardtags.Weapon];

    // handles the 'next weapon attack does potent damage'. copying this to MartialWeaponCard frfr
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != this || dealer != Owner.Creature)
        {
            return 0;
        }
        decimal bonus = base.ModifyDamageAdditive(target, amount, props, dealer, cardSource, cardPlay);
        if (Keywords.Contains(CoreKeywords.Potent))
        {
            bonus += Owner.PlayerCombatState?.GetPotency() ?? 0;
        }
        return bonus;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await base.AfterCardPlayed(choiceContext, cardPlay);

        if (cardPlay.Card == this && Keywords.Contains(CoreKeywords.Potent))
        {
            RemoveKeyword(CoreKeywords.Potent);
        }
    }
}