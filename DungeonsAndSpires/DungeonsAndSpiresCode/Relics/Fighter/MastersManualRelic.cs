using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;

[Pool(typeof(FighterRelicPool))]
// Maneuver cards spawn upgraded, gain a random maneuver every turn
public class MastersManualRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator != Owner)
            return;
        if (card is ManeuverCard maneuverCard)
        {
            CardCmd.Upgrade(maneuverCard);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if(player != Owner)
            return;
        var options = new List<CardModel>
        {
            ModelDb.Card<BaitAndSwitch>(),
            ModelDb.Card<QuickToss>(),
            ModelDb.Card<MenacingAttack>()
        };
        CardModel chosen = Owner.RunState.Rng.CombatCardGeneration.NextItem(options);
        var card = Owner.Creature.CombatState.CreateCard(chosen, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
    }
}