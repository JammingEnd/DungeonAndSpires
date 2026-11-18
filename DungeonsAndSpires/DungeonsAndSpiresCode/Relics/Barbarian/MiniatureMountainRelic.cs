using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;

[Pool(typeof(BarbarianRelicPool))]
// When you attack an enemy without reverb, apply one reverb to it.
public class MiniatureMountainRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner || cardPlay.Card.Type != CardType.Attack || cardPlay.Target == null || cardPlay.Target.IsPlayer)
        {
            return;
        }

        if (cardPlay.Target.GetPowerAmount<Reverberation>() > 0)
        {
            return;
        }

        await PowerCmd.Apply<Reverberation>(choiceContext, cardPlay.Target, 1, Owner.Creature, cardPlay.Card);
    }
}
