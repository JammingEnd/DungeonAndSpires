using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

// Shared Dart generation, so Blowpipe Repurpose can lock it for a turn.
public static class BlowpipeDarts
{
    public static bool GenerationLocked(Player owner) =>
        owner.Creature.GetPower<BlowpipeRepurposePower>() != null;

    public static async Task AddDart(ICombatState combatState, Player owner, PileType pile, CardPilePosition position, float previewSeconds = 0f)
    {
        if (GenerationLocked(owner))
        {
            return;
        }

        var dart = combatState.CreateCard<Dart>(owner);
        var added = await CardPileCmd.AddGeneratedCardToCombat(dart, pile, owner, position);

        if (previewSeconds > 0f)
        {
            CardCmd.PreviewCardPileAdd(added, previewSeconds);
        }
    }
}
