using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class SpearSpareTime() : MartialWeaponCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<SpearTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("DipUses", 2),
        new IntVar("DipStrength", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawPile = PileType.Draw.GetPile(Owner);
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-SPEAR_SPARE_TIME.selectionPrompt"), 0, 2);
        Func<CardModel, bool> filter = c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack && !c.HasDip();
        var selected = await CardSelectCmd.FromCombatPile(choiceContext, drawPile, Owner, prefs, filter);

        foreach (var card in selected)
        {
            var dip = DipExtentions.GenerateRandomDip(DynamicVars["DipUses"].IntValue, Owner, DynamicVars["DipStrength"].IntValue);
            await DipCmd.Add(choiceContext, Owner, card, dip);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DipStrength"].UpgradeValueBy(2m);
    }
}
