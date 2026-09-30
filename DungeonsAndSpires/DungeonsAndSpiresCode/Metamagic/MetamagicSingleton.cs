using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

public class MetamagicSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        int magik = player.PlayerCombatState.GetMetamagic();
        if (magik is >= 2 and < 4)
        {
            var newInt = player.RunState.Rng.CombatCardGeneration.NextInt(2);
            CardModel newCard;
            if (newInt == 0)
            {
                 newCard = player.Creature.CombatState.CreateCard<QuickendSpell>(player);
            }
            else
            {
                 newCard = player.Creature.CombatState.CreateCard<HeightenedSpell>(player);
            }
            await CardPileCmd.AddGeneratedCardToCombat(newCard, PileType.Hand, player);
        }
        else if (magik >= 4)
        {
            var quickend = player.Creature.CombatState.CreateCard<QuickendSpell>(player);
            var heightened = player.Creature.CombatState.CreateCard<HeightenedSpell>(player);
            await CardPileCmd.AddGeneratedCardToCombat(quickend, PileType.Hand, player);
            await CardPileCmd.AddGeneratedCardToCombat(heightened, PileType.Hand, player);
        }
       await MetamagicCmd.Set(choiceContext, player, 4);
    }
}