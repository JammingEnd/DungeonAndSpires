using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class MaulThunderBlast() : MartialWeaponCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.TwoHanded, CardKeyword.Exhaust];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<MaulTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar(4),
        new IntVar("DipUses", 3),
        new IntVar("DipStrength", 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var drawn = await CardPileCmd.Draw(choiceContext, DynamicVars.Cards.IntValue, Owner);
        foreach (var card in drawn)
        {
            if (card.Tags.Contains(DASCoreCardtags.Weapon) && card.Type == CardType.Attack)
            {
                await DipCmd.Add(choiceContext, Owner, card,
                    new DipInfo(CoreKeywords.Thunder, DynamicVars["DipStrength"].IntValue, DynamicVars["DipUses"].IntValue));
            }
        }

        await PowerCmd.Apply<Reverberation>(choiceContext, CombatState.HittableEnemies!, 1, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip,
        HoverTipFactory.FromPower<Reverberation>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DipStrength"].UpgradeValueBy(2);
    }
}
