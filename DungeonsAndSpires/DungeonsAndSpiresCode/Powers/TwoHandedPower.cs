using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public abstract class TwoHandedPower : DungeonsAndSpiresPower
{
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer,
        CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource is not MartialWeaponCard card || card.Owner?.Creature != Owner)
        {
            return 1m;
        }
        if (!card.Keywords.Contains(CoreKeywords.TwoHanded))
        {
            return 1m;
        }
        // Only the weapon family this power represents gets the 20% bonus.
        return card.TwoHandedPower?.Id == Id ? 1.2m : 1m;
    }
}

public class HalberdTwoHandedPower : TwoHandedPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
public class MaulTwoHandedPower : TwoHandedPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
public class SpearTwoHandedPower : TwoHandedPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
public class GreatswordTwoHandedPower : TwoHandedPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;
}
