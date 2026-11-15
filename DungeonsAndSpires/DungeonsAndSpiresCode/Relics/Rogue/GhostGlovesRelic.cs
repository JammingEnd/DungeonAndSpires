using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;

[Pool(typeof(RogueRelicPool))]
//When Hidden, spells do not generate Exhaustion  
public class GhostGlovesRelic : DungeonsAndSpiresRelic, IShouldCreateExhaustion
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public bool ShouldCreateExhaustion(SpellCard spell, PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return true;
        }

        return player.Creature.GetPower<RogueHiddenPower>() == null;
    }
}
