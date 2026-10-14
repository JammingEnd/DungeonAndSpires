using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class DaggerLastResortPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult result, ValueProp props, Creature dealer, CardModel cardSource)
    {
        if (target != Owner || result.UnblockedDamage <= 0)
        {
            return;
        }

        Func<CardModel, bool> filter = c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack && !c.HasDip();
        var weaponAttCards = Owner.Player.PlayerCombatState.AllCards.Where(filter);
        for (int i = 0; i < Amount; i++)
        {
            var card = Owner.Player.RunState.Rng.CombatCardSelection.NextItem(weaponAttCards);
            var dip = DipExtentions.GenerateRandomDip(1, card.Owner, 5);
            await DipCmd.Add(choiceContext, card.Owner,  card, dip);
        }
    }
}