using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

public class TwoHandedHandler() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var wanted = (cardPlay.Card as MartialWeaponCard)?.TwoHandedPower;
        var creature = cardPlay.Player.Creature;
        var current = creature.Powers.OfType<TwoHandedPower>().FirstOrDefault();

        if (current != null && wanted != null && current.Id == wanted.Id)
        {
            return;
        }

        if (current != null)
        {
            await PowerCmd.Remove(current);
        }

        if (wanted != null)
        {
            await PowerCmd.Apply(choiceContext, wanted, creature, 1, creature, cardPlay.Card);
        }
    }
}
