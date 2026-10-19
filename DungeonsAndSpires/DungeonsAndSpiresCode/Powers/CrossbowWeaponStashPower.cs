using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

public class CrossbowWeaponStashPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player)
        {
            return;
        }

        var drawPile = player.PlayerCombatState?.DrawPile.Cards;
        if (drawPile == null)
        {
            return;
        }

        var candidates = drawPile
            .Where(c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack && c.Keywords.Contains(CoreKeywords.Piering))
            .ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        var pick = player.RunState.Rng.CombatCardSelection.NextItem<CardModel>(candidates);
        if (pick != null)
        {
            await CardPileCmd.Add(pick, PileType.Hand);
        }
    }
}
