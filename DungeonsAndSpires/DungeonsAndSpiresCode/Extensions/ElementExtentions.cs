using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;

public static class ElementExtentions
{
    public static bool HasElementKeyword(this CardModel card)
    {
        if (card.Keywords.Any(x =>
                x == CoreKeywords.Acid ||
                x == CoreKeywords.Cold ||
                x == CoreKeywords.Fire ||
                x == CoreKeywords.Lightning ||
                x == CoreKeywords.Poison ||
                x == CoreKeywords.Thunder)) 
            return true;
        
        return false;
    }
}