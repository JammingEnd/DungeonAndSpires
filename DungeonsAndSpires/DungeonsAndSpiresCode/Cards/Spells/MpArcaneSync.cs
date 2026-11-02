using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;

// At the end of your turn, gain a spellslots back equal to the amount of cards each ally played that turn (so another player playing 6 cards give you a 6th level spellslot)
public class MpArcaneSync() : SpellCard(0, 1, CardType.Power, CardRarity.Rare, TargetType.Self)
{ 
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ArcaneSyncPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
    }
}
