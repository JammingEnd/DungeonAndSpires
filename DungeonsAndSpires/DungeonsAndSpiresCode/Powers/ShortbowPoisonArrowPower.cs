using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class ShortbowPoisonArrowPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    // 
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props,
        Creature? dealer, CardModel? cardSource)
    {
        if (target != Owner)
            return;
        if(cardSource == null && cardSource is SpellCard)
            return;
        if(!cardSource.Tags.Contains(DASCoreCardtags.Weapon))
            return;
        DipInfo newDip = DipExtentions.GenerateDip(CoreKeywords.Poison, Amount, 1);
        await DipCmd.Add(choiceContext, cardSource.Owner, cardSource, newDip);
        await PowerCmd.ModifyAmount(choiceContext, this, -Amount, null, null);
    }
}