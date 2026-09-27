using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class CrownOfStarsPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.Instanced;

    private bool _giveUpgraded;

    public void IsUpgraded(bool giveUpgraded)
    {
        _giveUpgraded = giveUpgraded;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        for (int i = 0; i < 2; i++)
        {
            var strike = CombatState.CreateCard(ModelDb.Card<StarStrike>(), player);
            if (_giveUpgraded)
            {
                strike.UpgradeInternal();
            }
            await CardPileCmd.AddGeneratedCardToCombat(strike, PileType.Hand, player);
        }

        decimal newAmount = await PowerCmd.ModifyAmount(choiceContext, this, -1, null, null);
        if (newAmount <= 0)
        {
            await PowerCmd.Remove(this);
        }
    }
}