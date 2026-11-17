using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

// lvl1: protect weaker allies at your own cost. lvl2: feed energy to allies with less HP than you.
public class WolfSpirit : SpiritPower
{
    private bool HasWeakerAlly()
    {
        var players = Owner.CombatState?.Players;
        if (players == null)
        {
            return false;
        }

        return players.Any(p => p.Creature != Owner && p.Creature.IsAlive && p.Creature.CurrentHp < Owner.CurrentHp);
    }

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (_level < 1 || target == null)
        {
            return 1m;
        }

        if (target.IsPlayer && target != Owner && target.CurrentHp < Owner.CurrentHp)
        {
            return 0.75m; // damage to allies is lessened
        }

        if (target == Owner && HasWeakerAlly())
        {
            return 1.15m; // you take more
        }

        return 1m;
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || _level < 2)
        {
            return;
        }

        var players = Owner.CombatState?.Players;
        if (players == null)
        {
            return;
        }

        foreach (var ally in players)
        {
            if (ally.Creature != Owner && ally.Creature.IsAlive && ally.Creature.CurrentHp < Owner.CurrentHp)
            {
                await PlayerCmd.GainEnergy(1, ally);
            }
        }
    }
}
