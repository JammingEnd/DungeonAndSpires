using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;

[Pool(typeof(RogueRelicPool))]
// whenever you enter Hidden, upgrade a random card in your hand
public class GoldenKeyRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (amount <= 0 || power is not RogueHiddenPower || power.Owner != Owner.Creature)
        {
            return;
        }

        var hand = Owner.PlayerCombatState?.Hand.Cards.Where(c => !c.IsUpgraded).ToList();
        if (hand == null || hand.Count == 0)
        {
            return;
        }

        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(hand);
        CardCmd.Upgrade(card);
    }
}
