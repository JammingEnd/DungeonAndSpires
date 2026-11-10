using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// increases a random debuff on this enemy by one when attacked.
public class SlingshotGruesomePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;
    public override PowerInstanceType InstanceType => PowerInstanceType.InstancedPerApplier;

    private CardModel? _source;

    public void SetSource(CardModel? source) => _source = source;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == _source || cardPlay.Card.Type != CardType.Attack || cardPlay.Target != Owner)
        {
            return;
        }

        if (cardPlay.Player != Applier?.Player)
        {
            return;
        }

        var debuffs = Owner.Powers.Where(p => p.Type == PowerType.Debuff && p != this && p.StackType == PowerStackType.Counter).ToList();
        if (debuffs.Count > 0)
        {
            var debuff = cardPlay.Player.RunState.Rng.CombatCardSelection.NextItem(debuffs);
            await PowerCmd.ModifyAmount(choiceContext, debuff, 1, Applier, null);
        }

        await PowerCmd.Decrement(this);
    }
}
