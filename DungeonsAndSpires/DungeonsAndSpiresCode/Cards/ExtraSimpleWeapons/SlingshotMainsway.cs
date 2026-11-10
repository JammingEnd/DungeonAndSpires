using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotMainsway() : ExtraSimpleWeaponCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Uses", 2),
        new IntVar("Strength", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        if (drawPile.Cards.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-SPEAR_SPARE_TIME.selectionPrompt"), 0, 1);
        Func<CardModel, bool> filter = c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack && !c.HasDip();
        var selected = await CardSelectCmd.FromCombatPile(choiceContext, drawPile, Owner, prefs, filter);
        var card = selected.FirstOrDefault();
        if (card == null)
        {
            return;
        }

        var dip = DipExtentions.GenerateRandomDip(DynamicVars["Uses"].IntValue, Owner, DynamicVars["Strength"].IntValue);
        await DipCmd.Add(choiceContext, Owner, card, dip);
        await CardPileCmd.Add(card, PileType.Draw, CardPilePosition.Top);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Strength"].UpgradeValueBy(1);
    }
}
