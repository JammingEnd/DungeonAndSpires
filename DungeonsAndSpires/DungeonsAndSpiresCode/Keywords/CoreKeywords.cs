using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;

public static class CoreKeywords
{
    // Potent keywords indicate that the card uses ability potency for some stats 
    [CustomEnum("Potent")] 
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Potent;
    
    // Ritual spells do not consume a spellslot when your energy is above 50% (when max is 3, its above 2)
    [CustomEnum("Ritual")] 
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Ritual;
    
    // scrolled can only be applied to spell, at which they gain exhaust and are free to play. not on cantrips
    [CustomEnum("Scrolled")] 
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Scrolled;
    
    // Finesse weapons may also use dex to add to its damage, whichever is highest
    [CustomEnum("Finesse")] 
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword Finesse;
    
    // Two handed weapons deal 20% more damage on subsequent attacks with the same weapon.
    [CustomEnum("TwoHanded")] 
    [KeywordProperties(AutoKeywordPosition.Before)]
    public static CardKeyword TwoHanded;
    
    // ======= Damage types
    [CustomEnum("Fire")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Fire;
    
    [CustomEnum("Cold")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Cold;
    
    [CustomEnum("Thunder")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Thunder;
    
    [CustomEnum("Lightning")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Lightning;
    
    [CustomEnum("Piercing")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Piering;
    
    [CustomEnum("Slashing")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Slashing;
    
    [CustomEnum("Bludgeoning")] 
    [KeywordProperties(AutoKeywordPosition.After)]
    public static CardKeyword Bludgeoning;
}