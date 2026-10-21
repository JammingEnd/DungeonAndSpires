using BaseLib.Commands;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class ShortswordSwitchHands() : SimpleWeaponCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(11, ValueProp.Move),
        new CardsVar("Discard", 2),
        new CardsVar("Draw", 2),
        new IntVar("Scry", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discardPrefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-SHORTSWORD_SWITCH_HANDS.discardPrompt"), 0, DynamicVars["Discard"].IntValue);
        var toDiscard = await CardSelectCmd.FromHand(choiceContext, Owner, discardPrefs, null, this);
        foreach (var card in toDiscard)
        {
            await CardCmd.Discard(choiceContext, card);
        }

        await ScryCmd.Execute(choiceContext, Owner, DynamicVars["Scry"].IntValue);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Draw"].IntValue, Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Discard"].UpgradeValueBy(1);
        DynamicVars["Scry"].UpgradeValueBy(1);
    }
}