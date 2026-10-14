using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class QuarterstaffPoleStorage() : SimpleWeaponCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Bludgeoning
    ];

    // The dip types the player can choose from, same set Chromatic Orb offers.
    public override IEnumerable<CardKeyword> ElementOptions =>
    [
        CoreKeywords.Fire,
        CoreKeywords.Cold,
        CoreKeywords.Thunder,
        CoreKeywords.Lightning,
        CoreKeywords.Poison
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(4, ValueProp.Move),
        new IntVar("DipTurns", 1),
        new IntVar("DipStrength", 2),
        new PowerVar<PoisonPower>("PoisonPower", 2),
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        CommonActions.CardBlock(this, cardPlay);

        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-QUARTERSTAFF_POLE_STORAGE.selectionPrompt"), 1, 1);
        Func<CardModel, bool> filter = c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack;
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, filter, this);
        var target = selected.FirstOrDefault();
        if (target == null)
        {
            return;
        }

        var type = await SelectElement(choiceContext);
        if (type is not { } dipType)
        {
            return;
        }

        await DipCmd.Add(choiceContext, Owner, target,
            new DipInfo(dipType, DynamicVars["DipStrength"].IntValue, DynamicVars["DipTurns"].IntValue));
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Bludgeoning)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["DipStrength"].UpgradeValueBy(2m);
    }
}