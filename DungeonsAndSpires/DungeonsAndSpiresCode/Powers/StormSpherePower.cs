using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class StormSpherePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        for (int i = 0; i < (int)Amount; i++)
        {
            var strike = CombatState.CreateCard(ModelDb.Card<LightningStrike>(), player);
            await CardPileCmd.AddGeneratedCardToCombat(strike, PileType.Hand, player);
        }
    }
}