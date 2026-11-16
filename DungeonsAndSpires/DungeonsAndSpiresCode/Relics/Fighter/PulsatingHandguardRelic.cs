using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;

[Pool(typeof(FighterRelicPool))]
// After gaining Second wind, a random card gains +replay 1  
public class PulsatingHandguardRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (applier != Owner.Creature || power is not SecondWindPower)
            return;

        var selection = Owner.PlayerCombatState.AllCards.Where(c =>
            c.Type is not (CardType.Curse or CardType.Status or CardType.None or CardType.Quest));
       var card = selection.TakeRandom( 1, Owner.RunState.Rng.CombatCardSelection).FirstOrDefault();
       if (card != null)
       {
           card.BaseReplayCount += 1;
           CardCmd.Preview(card, 0.8f);
       }
    }
}