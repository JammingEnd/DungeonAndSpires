using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public class DipState
{
   public Dictionary<CardModel, DipInfo> WeaponDips =  new Dictionary<CardModel, DipInfo>();
}

public class DipInfo
{
    public CardKeyword Type { get; private  set; }
    public int Strength { get; private set; }
    public int UsesLeft { get; private set; }

    public DipInfo(CardKeyword type, int strength, int duration)
    {
        Type = type;
        Strength = strength;
        UsesLeft = duration;
    }

    public bool DecreaseDuration()
    {
        UsesLeft -= 1;
        return UsesLeft <= 0;
    }

    public void IncreaseDuration(int amount)
    {
        UsesLeft += amount;
    }

    public void ModifyStrength(int amount)
    {
        Strength += amount;
    }
}