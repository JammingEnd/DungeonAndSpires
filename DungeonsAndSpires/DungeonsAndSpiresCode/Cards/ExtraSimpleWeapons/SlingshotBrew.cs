using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotBrew() : ExtraSimpleWeaponCard(0, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Uses", 2),
        new IntVar("Strength", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var candidates = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack && !c.HasDip()).ToList();

        if (candidates.Count == 0)
        {
            return;
        }

        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(candidates);
        var dip = DipExtentions.GenerateRandomDip(DynamicVars["Uses"].IntValue, Owner, DynamicVars["Strength"].IntValue);
        await DipCmd.Add(choiceContext, Owner, card, dip);
    }
    
    public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (card == this)
        {
            DynamicVars["Strength"].BaseValue += 1m;
        }
        return Task.CompletedTask;
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Uses"].UpgradeValueBy(1);
    }
}
