using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
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

public class MaceRuinEtch() : ExtraSimpleWeaponCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    private static readonly CardKeyword[] EtchTypes =
    [
        CoreKeywords.Cold,
        CoreKeywords.Lightning,
        CoreKeywords.Thunder
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Dips", 2),
        new IntVar("Uses", 2),
        new IntVar("Strength", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-SPEAR_SPARE_TIME.selectionPrompt"), 0, DynamicVars["Dips"].IntValue);
        Func<CardModel, bool> filter = c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack && !c.HasDip();
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, filter, this);

        foreach (var card in selected)
        {
            var type = EtchTypes[Owner.RunState.Rng.CombatCardSelection.NextInt(EtchTypes.Length)];
            await DipCmd.Add(choiceContext, Owner, card, new DipInfo(type, DynamicVars["Strength"].IntValue, DynamicVars["Uses"].IntValue));
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Dips"].UpgradeValueBy(1);
        DynamicVars["Strength"].UpgradeValueBy(1);
    }
}
