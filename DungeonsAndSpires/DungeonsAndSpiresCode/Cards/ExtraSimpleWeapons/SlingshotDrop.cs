using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotDrop() : ExtraSimpleWeaponCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, DynamicVars.Cards.IntValue);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);

        if (selected.Any())
        {
            await CardCmd.Discard(choiceContext, selected);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
