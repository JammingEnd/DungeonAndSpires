using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;

[Pool(typeof(FighterRelicPool))]
public class BattleMasterRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    public override async Task InitialBonus()
    {
        
    }

    public override async Task Act2Bonus()
    {
        
    }

    public override async Task Act3Bonus()
    {
    
    }
}