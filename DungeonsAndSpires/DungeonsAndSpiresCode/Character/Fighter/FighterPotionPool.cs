using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using Godot;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class FighterPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => FighterCharacter.Color;

    public override string BigEnergyIconPath => "charui/Core/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/Core/text_energy.png".ImagePath();
}
