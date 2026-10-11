using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;

public class AddDipInfoToCards
{
    private static readonly AddedNode<NCard, Control> _addedNode = new(
        "res://DungeonsAndSpires/images/cards/DipCardInfo.tscn",
        (card, display) =>
        {
            display.Visible = false;

            var cardContainer = card.GetChild(0)!;
            cardContainer.AddChild(display);

            //Changing position to before the star icon node.
            cardContainer.MoveChild(display, cardContainer.GetNode("%StarIcon").GetIndex());

            //Keep track of the display so DipCmd.SetUI can find it by card model.
            if (card.Model != null)
            {
                DipField.UI[card.Model] = display;
                DipCmd.SetUI(card.Model, true);
            }
        });
}