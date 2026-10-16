using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

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

        // Switching families: Switch keeps the bonus alive and is spent.
        var switchPower = creature.GetPower<SwitchPower>();
        if (current != null && switchPower != null)
        {
            await PowerCmd.ModifyAmount(choiceContext, switchPower, -1, null, null);
        }

        if (current != null)
        {
            await PowerCmd.Remove(current);
        }

        if (wanted != null)
        {
            await PowerCmd.Apply(choiceContext, (PowerModel)wanted.ToMutable(), creature, 1, creature, cardPlay.Card);
        }
    }
}
