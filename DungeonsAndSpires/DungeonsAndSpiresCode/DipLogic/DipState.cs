using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public class DipState
{
   public Dictionary<CardModel, DipInfo> WeaponDips =  new Dictionary<CardModel, DipInfo>();
}

public class DipInfo(CardKeyword type, int strength, int duration)
{
    public CardKeyword Type { get; private  set; } = type;
    public int Strength { get; private set; } = strength;
    public int UsesLeft { get; private set; } = duration;

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