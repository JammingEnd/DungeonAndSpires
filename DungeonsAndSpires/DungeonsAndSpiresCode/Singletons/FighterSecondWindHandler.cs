using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

public class FighterSecondWindHandler() : CustomSingletonModel(HookType.Combat)
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

            await PowerCmd.Apply<FighterSecondWindPower>(new BlockingPlayerChoiceContext(), player.Creature,
                FighterSecondWindPower.AttacksRequired, player.Creature, null);
        }
    }
}
