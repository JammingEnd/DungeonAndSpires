using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Warlock;

// Gain Block, then gain Thorns that deal Cold damage to enemies that hit you while you have Block.
[Pool(typeof(WarlockCardPool))]
public class ArmorOfAgathys() : SpellCard(1, 2, CardType.Skill, CardRarity.Basic, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Cold];

    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Thorns", (1, 2) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(8, ValueProp.Move),
        new IntVar("Thorns", 4)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.IntValue, ValueProp.Unpowered, cardPlay);
        await PowerCmd.Apply<ArmorOfAgathysPower>(choiceContext, Owner.Creature, DynamicVars["Thorns"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<ArmorOfAgathysPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(2m);
        DynamicVars["ThornsBase"].UpgradeValueBy(2m);
        DynamicVars["ThornsExtra"].UpgradeValueBy(1m);
    }
}
