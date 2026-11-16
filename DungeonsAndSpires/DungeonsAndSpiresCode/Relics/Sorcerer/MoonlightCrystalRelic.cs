using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Sorcerer;

[Pool(typeof(SorcererRelicPool))]
// cards with your metamagic cannot be exhausted
public class MoonlightCrystalRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override CardLocation ModifyCardPlayResultLocation(CardModel card, bool isAutoPlay, ResourceInfo resources,
        CardLocation cardLocation)
    {
        if (card.Owner != Owner)
            return cardLocation;
        if (cardLocation.pileType == PileType.Exhaust && (card.Keywords.Contains(CoreKeywords.Heightened) || card.Keywords.Contains(CoreKeywords.Quickened)))
        {
            var cardLoc = cardLocation;
            cardLoc.pileType = PileType.Discard;
            return cardLoc;
        }
        return cardLocation;
    }
}