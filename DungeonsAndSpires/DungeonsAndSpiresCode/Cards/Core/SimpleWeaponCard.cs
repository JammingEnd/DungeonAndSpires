using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

[Pool(typeof(DASSimpleWeaponCardPool))]
public abstract class SimpleWeaponCard(int cost, CardType type, CardRarity rarity, TargetType target) : CoreCard(cost, type, rarity, target)
{
    
}