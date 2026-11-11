using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class RogueHidePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("max", 7)];
    private int _stacks;
    public override int DisplayAmount => _stacks;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner.Player)
            return;
        var card = cardPlay.Card;
        if (card.Type == CardType.Attack && card.EnergyCost.GetResolved() > 1)
        {
            Decrease();
        }
        else if (card.Type == CardType.Power || card.Type == CardType.Skill || card.Type == CardType.Attack)
        {
            _stacks++;
            if (_stacks >= DynamicVars["max"].IntValue)
            {
                _stacks = 0;
                await PowerCmd.Apply<RogueHiddenPower>(choiceContext, Owner, 1, Owner, null);
            }
        }

        InvokeDisplayAmountChanged();
    }
    
    void Decrease()
    {
        if (_stacks <= 0)
            return;
        _stacks--;
    }
    
}