using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;


[Pool(typeof(FighterRelicPool))]
public class EchoKnightRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    public override Task InitialBonus()
    {
        throw new NotImplementedException();
    }

    public override Task Act2Bonus()
    {
        throw new NotImplementedException();
    }

    public override Task Act3Bonus()
    {
        throw new NotImplementedException();
    }
}