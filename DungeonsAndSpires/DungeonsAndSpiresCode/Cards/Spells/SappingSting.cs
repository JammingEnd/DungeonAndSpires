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

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class SappingSting() : SpellCard(0, 1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Poison", (4, 3) },
        { "Weak", (4, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<PoisonPower>("Poison", 3),
        new PowerVar<WeakPower>("Weak", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool hadPoison = cardPlay.Target.GetPowerAmount<PoisonPower>() > 0;

        await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, DynamicVars["Poison"].GetCalculatedValue(), Owner.Creature, this);

        if (!hadPoison)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["Weak"].GetCalculatedValue(), Owner.Creature, this);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["PoisonBase"].UpgradeValueBy(1m);
        DynamicVars["PoisonExtra"].UpgradeValueBy(1m);
    }
}