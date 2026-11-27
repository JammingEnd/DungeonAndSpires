using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Warlock;

public class GreatOneRelic : SubclassRelic
{
    public override RelicRarity Rarity { get; }
    public override async Task InitialBonus() { }

    public override async Task Act2Bonus()
    {
        //TODO: otherworldy patron slots
    }

    public override async Task Act3Bonus()
    {
        _cantripsCasted = 0;
        InvokeDisplayAmountChanged();
    }

    private int _cantripsCasted;
    public override int DisplayAmount => _cantripsCasted;

    // act1, playing cantrips that cost energy draws a card
    // act3, after playing 3 cantrips, gain a spellslot
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;
        if (cardPlay.Card.Tags.Contains(DASCoreCardtags.Cantrip))
        {
            if (this.CurrentLevel >= 3)
            {
                if(_cantripsCasted == 3)
                    _cantripsCasted = 0;
                
                _cantripsCasted++;
                InvokeDisplayAmountChanged();
                await SpellslotsCmd.AddSpellSlotForLevel(choiceContext, cardPlay.Player, 1);
            }
            if(cardPlay.Card.EnergyCost.GetResolved() > 0) 
                await CardPileCmd.Draw(choiceContext, cardPlay.Card.Owner);
        }
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<EmpowerPatronPower>(),
    ];
}