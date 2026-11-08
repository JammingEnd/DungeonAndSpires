using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class BlowpipeIvyPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        var player = Owner.Player;
        if (player == null || card is not Dart || card.Owner != player || card.Owner.PlayerCombatState?.GetDip(card) != null)
        {
            return;
        }

        await DipCmd.Add(new BlockingPlayerChoiceContext(), player, card, new DipInfo(CoreKeywords.Poison, Amount, 1));
    }
}
