using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class BlowpipeScavengePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;
    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("left", 3)];

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card.Owner != Owner.Player || card is not Dart)
        {
            return;
        }

        var playsLeft = DynamicVars["left"];
        
        playsLeft.BaseValue--;
        if (playsLeft.BaseValue == 1)
        {
            this.Flash();
        }
        if (playsLeft.BaseValue % 3 != 0)
        {
            return;
        }

        var combatState = Owner.CombatState;
        if (combatState == null)
        {
            return;
        }

        var cards = Owner.Player.PlayerCombatState.AllCards.Where(c => c.HasDip() && c.Pile.Type != PileType.Exhaust);
        var selected = combatState.RunState.Rng.CombatCardSelection.NextItem(cards);
        playsLeft.BaseValue = 3;
        
        if(selected == null)
            return;
        
        await DipCmd.ModifyStrength(choiceContext, selected.Owner, selected, Amount);
        CardCmd.Preview(selected, 0.3f, CardPreviewStyle.MessyLayout);
    }
}
