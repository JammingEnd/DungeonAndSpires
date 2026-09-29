using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class FrostbitePower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    private static readonly CardKeyword[] VulnerableTypes =
    [
        CoreKeywords.Thunder,
        CoreKeywords.Lightning,
        CoreKeywords.Bludgeoning
    ];

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner || cardSource == null)
        {
            return 1m;
        }
        if (!VulnerableTypes.Any(type => cardSource.Keywords.Contains(type)))
        {
            return 1m;
        }
        return 1.25m;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        await Decrement(choiceContext);
    }

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target != Owner || cardSource == null)
        {
            return;
        }
        if (cardSource.Keywords.Contains(CoreKeywords.Fire))
        {
            await Decrement(choiceContext);
        }
    }

    private async Task Decrement(PlayerChoiceContext choiceContext)
    {
        if (Amount <= 0)
        {
            return;
        }

        await PowerCmd.Decrement(this);
    }
}