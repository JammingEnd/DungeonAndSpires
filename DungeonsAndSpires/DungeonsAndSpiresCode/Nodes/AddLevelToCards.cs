using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;

public class AddLevelToCards
{
    private static AddedNode<NCard, SpellLevelControl> AddedNode => new(
        "res://DungeonsAndSpires/images/cards/spellLevel.tscn",  
        (card, display) =>
        {
            display.Visible = false;
            Label label = (Label)display.GetChild(1);

            if (card.Model is SpellCard spellCard)
            {
                label.Text = ToNumeral(spellCard.Level);
                display.Visible = true;
            }
        
            var cardContainer = card.GetChild(0)!;
            cardContainer.AddChild(display);

            //Changing position to before the star icon node.
            cardContainer.MoveChild(display, cardContainer.GetNode("%StarIcon").GetIndex());
        });
    private static string ToNumeral(int level)
    {
        switch (level)
        {
            case 0: return "Cantrip";
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "VI";
            case 7: return "VII";
            case 8: return "VIII";
            case 9: return "IX";
        }
        return level.ToString();
    }
}