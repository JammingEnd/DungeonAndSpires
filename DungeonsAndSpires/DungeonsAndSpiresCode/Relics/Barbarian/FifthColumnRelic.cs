using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;

[Pool(typeof(BarbarianRelicPool))]
// Upon reaching 5 rage, gain another random Spirit.
public class FifthColumnRelic : DungeonsAndSpiresRelic
{
    public override RelicRarity Rarity => RelicRarity.Ancient;
    
    private bool _granted;

    public override Task BeforeCombatStart()
    {
        _granted = false;
        return base.BeforeCombatStart();
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || _granted)
        {
            return;
        }

        var rage = Owner.Creature.GetPower<BarbarianRagePower>();
        if (rage == null || rage.Amount < 5)
        {
            return;
        }

        _granted = true;

        List<SpiritPower> spirits =
        [
            ModelDb.Power<BearSpirit>(),
            ModelDb.Power<EagleSpirit>(),
            ModelDb.Power<ElkSpirit>()
        ];
        if (Owner.Creature.CombatState.Players.Count > 1)
        {
            spirits.Add(ModelDb.Power<WolfSpirit>());
        }
        // shoo, go away dupe
        spirits.Remove(Owner.Creature.GetPower<SpiritPower>());
        var spirit = (SpiritPower)spirits[Owner.RunState.Rng.CombatCardGeneration.NextInt(spirits.Count)].ToMutable();
        spirit.SetLevel(1);
        await PowerCmd.Apply(choiceContext, spirit, Owner.Creature, 1, Owner.Creature, null);
    }
}
