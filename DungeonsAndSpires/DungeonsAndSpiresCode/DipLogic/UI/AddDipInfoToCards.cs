using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;

public class AddDipInfoToCards
{
    // Exposed so the NCard.Reload postfix can reach each card's control via the SpireField syntax.
    internal static readonly AddedNode<NCard, DipInfoControl> AddedNode = new(
        "res://DungeonsAndSpires/images/cards/DipCardInfo.tscn",
        (card, display) =>
        {
            display.Visible = false;

            var cardContainer = card.GetChild(0)!;
            cardContainer.AddChild(display);

            //Changing position to before the star icon node.
            cardContainer.MoveChild(display, cardContainer.GetNode("%StarIcon").GetIndex());

            display.Initialize(card);
        });
}