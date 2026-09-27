using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class SimulacrumPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var state = player.PlayerCombatState?.GetSpellslotsState();
        if (state == null)
        {
            return;
        }
        
        // getting total missing spellslots
        var missing = new List<int>();
        foreach (var (level, slot) in state.Spellslots)
        {
            int missingCount = slot.GetMax() - slot.GetCurrent();
            for (int i = 0; i < missingCount; i++)
            {
                missing.Add(level);
            }
        }

        int amount = Amount;
        while (amount > 0 && missing.Count > 0)
        {
            int index = player.RunState.Rng.CombatTargets.NextInt(missing.Count);
            await SpellslotsCmd.AddSpellSlotForLevel(choiceContext, player, missing[index]);
            missing.RemoveAt(index);
            amount--;
        }
    }
}