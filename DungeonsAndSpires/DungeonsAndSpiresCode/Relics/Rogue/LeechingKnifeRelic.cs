using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;

[Pool(typeof(RogueRelicPool))]
// If an enemy dies on the first turn, Play your most expensive power
public class LeechingKnifeRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;

    private bool _isActive;
    
    public override async Task AfterDeath(PlayerChoiceContext choiceContext, Creature creature, CardModel? source, bool wasRemovalPrevented,
        float deathAnimLength)
    {
        if (Owner.PlayerCombatState.TurnNumber < 2 && creature.IsEnemy && _isActive)
        {
            var card = Owner.PlayerCombatState.AllCards.OrderByDescending(x => x.EnergyCost.GetResolved())
                .FirstOrDefault();
            if (card != null)
            {
                await CardCmd.AutoPlay(choiceContext, card, Owner.Creature);
                _isActive = false;
                this.Status = RelicStatus.Disabled;
            }
        }
    }

    public override Task BeforeCombatStart()
    {
        _isActive = true;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}