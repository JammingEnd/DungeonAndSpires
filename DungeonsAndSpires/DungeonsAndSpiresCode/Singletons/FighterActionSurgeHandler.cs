using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

public class FighterActionSurgeHandler() : CustomSingletonModel(HookType.Combat)
{
    public override async Task BeforeCombatStart()
    {
        var combatState = CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null)
        {
            return;
        }

        foreach (var player in combatState.Players)
        {
            if (player.Character.CardPool is not FighterCardPool)
            {
                continue;
            }

            var card = combatState.CreateCard<ActionSurge>(player);
            await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, player);
        }
    }
}
