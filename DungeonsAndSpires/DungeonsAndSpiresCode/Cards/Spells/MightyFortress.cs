using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class MightyFortress() : SpellCard(8, 2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Plating", (3, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<PlatingPower>("Plating", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<PlatingPower>(choiceContext, Owner.Creature, DynamicVars["Plating"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<PlatingPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["PlatingBase"].UpgradeValueBy(1m);
    }
}