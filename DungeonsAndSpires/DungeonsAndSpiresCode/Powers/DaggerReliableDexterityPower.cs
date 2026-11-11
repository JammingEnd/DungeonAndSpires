using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// Promised Dexterity arrives next turn and fades at the end of that turn.
public class DaggerReliableDexterityPower : CustomTemporaryPowerModelWrapper<DaggerReliablePower, DexterityPower>
{
    public override PowerType Type => PowerType.Buff;

    protected override bool InvertInternalPowerAmount => false;
}
