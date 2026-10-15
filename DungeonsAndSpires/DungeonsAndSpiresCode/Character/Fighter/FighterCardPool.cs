using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class FighterCardPool : CustomCardPoolModel
{
    public override string Title => FighterCharacter.CharacterId; //This is not a display name.

    public override string BigEnergyIconPath => "charui/Core/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/Core/text_energy.png".ImagePath();

    public override float H => 0.07f; //Hue; changes the color.
    public override float S => 1.3f; //Saturation
    public override float V => 1f; //Brightness

    public override Color DeckEntryCardColor => new("ffffff");

    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return ModelDb.CardPool<DASMartialWeaponCardPool>().Add(ModelDb.CardPool<DASSimpleWeaponCardPool>()).ToArray();
    }
}
