using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class MaceSharpen() : ExtraSimpleWeaponCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar(3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-MACE_SHARPEN.selectionPrompt"), 0, DynamicVars.Cards.IntValue);
        var selected = (await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this)).ToList();
        if (selected.Count == 0)
        {
            return;
        }

        foreach (var card in selected)
        {
            if (!card.IsUpgraded)
            {
                CardCmd.Upgrade(card);
            }
        }

        await CardCmd.Discard(choiceContext, selected);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
