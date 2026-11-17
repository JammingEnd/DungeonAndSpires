using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class ElkSpirit : SpiritPower
{
    private bool _usedThisTurn;
    private CardModel? _card;
    private int _damageDealt;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        _usedThisTurn = false;
        _card = null;
        _damageDealt = 0;

        if (_level >= 2 && Owner.CombatState?.HittableEnemies is { } enemies)
        {
            await PowerCmd.Apply<WeakPower>(choiceContext, enemies, 1, Owner, null);
        }
    }

    public override async Task AfterDamageGiven(PlayerChoiceContext choiceContext, Creature? dealer, DamageResult result, ValueProp props, Creature target, CardModel? cardSource)
    {
        if (_level < 1 || _usedThisTurn || dealer != Owner || cardSource == null || cardSource.Owner != Owner.Player
            || cardSource.Type != CardType.Attack || cardSource.EnergyCost.GetResolved() != 2)
        {
            return;
        }
        // for card with multihits
        if (_card != cardSource)
        {
            _card = cardSource;
            _damageDealt = 0;
        }

        _damageDealt += result.TotalDamage;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (_card != cardPlay.Card || _usedThisTurn || cardPlay.Player != Owner.Player)
        {
            return;
        }

        _usedThisTurn = true;
        int block = _damageDealt / 2;
        if (block > 0)
        {
            await CreatureCmd.GainBlock(Owner, block, ValueProp.Unpowered, null);
        }

        _card = null;
        _damageDealt = 0;
    }
}
