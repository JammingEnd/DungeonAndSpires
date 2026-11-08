using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class BlowpipeScavengePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private int _dartsPlayed;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player || cardPlay.Card is not Dart)
        {
            return;
        }

        _dartsPlayed++;
        if (_dartsPlayed % 2 != 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        for (int i = 0; i < Amount; i++)
        {
            var copy = combatState.CloneCard(cardPlay.Card);
            await CardPileCmd.Add(copy, PileType.Discard);
        }
    }
}
