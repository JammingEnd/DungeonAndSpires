using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;

[Pool(typeof(BarbarianRelicPool))]
// Weak has no effect on you while Raged. (Losing Weak instead of Rage is handled in BarbarianRagePower.)
public class HopiumRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner.Creature || target == dealer)
        {
            return 1m;
        }

        if (Owner.Creature.GetPower<BarbarianRagePower>() == null || dealer.GetPowerAmount<WeakPower>() <= 0 || !props.IsPoweredAttack())
        {
            return 1m;
        }
        
        return 4m / 3m;
    }
}
