using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// Coating a blade gives a little cover while you do it.
public class SlingshotEndurancePower : DungeonsAndSpiresPower, IOnDipChanged
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnDipChanged(PlayerChoiceContext choiceContext, Player player, CardModel card, DipInfo dipInfo)
    {
        // Add() stores the dip before the hook fires; Remove() clears it first.
        if (player != Owner.Player || !card.HasDip())
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner, Amount, ValueProp.Move, null);
    }
}
