using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Barbarian;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// Each turn, up to two Reckless in hand are hammered into AfterPunch.
public class BeautyInAngerPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var reckless = PileType.Hand.GetPile(player).Cards.OfType<Reckless>().Take(2).ToList();
        foreach (var card in reckless)
        {
            var punch = Owner.CombatState.CreateCard(ModelDb.Card<AfterPunch>(), player);
            await CardCmd.Transform(card, punch);
        }
    }
}
