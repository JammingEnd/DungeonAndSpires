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

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class ShortswordSwitchHands() : SimpleWeaponCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar("Discard", 1),
        new IntVar("Scry", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var discardPrefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-SHORTSWORD_SWITCH_HANDS.discardPrompt"), 0, DynamicVars["Discard"].IntValue);
        Func<CardModel, bool> weaponFilter = c => c.Tags.Contains(DASCoreCardtags.Weapon);
        var toDiscard = await CardSelectCmd.FromHand(choiceContext, Owner, discardPrefs, weaponFilter, this);
        foreach (var card in toDiscard)
        {
            await CardCmd.Discard(choiceContext, card);
        }

        await ScryCmd.Execute(choiceContext, Owner, DynamicVars["Scry"].IntValue);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Discard"].UpgradeValueBy(1);
        DynamicVars["Scry"].UpgradeValueBy(1);
    }
}