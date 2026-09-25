using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class MirrorImage() : SpellCard(2, 1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Block", (3, 4) },
        { "Cards", (3, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(9, ValueProp.Unpowered),
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].GetCalculatedValue(), ValueProp.Unpowered, cardPlay);
        await CardPileCmd.Draw(choiceContext, DynamicVars["Cards"].GetCalculatedValue(), Owner);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["BlockBase"].UpgradeValueBy(4m);
        DynamicVars["BlockExtra"].UpgradeValueBy(2m);
        DynamicVars["CardsBase"].UpgradeValueBy(1m);
    }
}