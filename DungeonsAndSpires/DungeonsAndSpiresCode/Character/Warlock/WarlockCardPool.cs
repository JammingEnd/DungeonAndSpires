using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class WarlockCardPool : CustomCardPoolModel
{
    public override string Title => WarlockCharacter.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/Core/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/Core/text_energy.png".ImagePath();


    /* These HSV values will determine the color of your card back.
    They are applied as a shader onto an already colored image,
    so it may take some experimentation to find a color you like.
    Generally they should be values between 0 and 1. */
    public override float H => 0.07f; //Hue; changes the color.
    public override float S => 1.3f; //Saturation
    public override float V => 1f; //Brightness
    

    //Color of small card icons
    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return ModelDb.CardPool<DASSpellCardPool>().Add(ModelDb.CardPool<DASSimpleWeaponCardPool>()).ToArray();
    }
}
