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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class ModifyMemory() : SpellCard(5, 2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Strength", (5, 4) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<StrengthPower>("Strength", 8)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<ModifyMemoryPower>(choiceContext, cardPlay.Target, DynamicVars["Strength"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent),
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthBase"].UpgradeValueBy(3m);
        DynamicVars["StrengthExtra"].UpgradeValueBy(1m);
    }
}