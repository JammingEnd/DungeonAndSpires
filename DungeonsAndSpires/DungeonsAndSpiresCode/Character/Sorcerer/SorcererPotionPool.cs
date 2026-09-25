using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using Godot;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class SorcererPotionPool : CustomPotionPoolModel
{
    public override Color LabOutlineColor => SorcererCharacter.Color;


    public override string BigEnergyIconPath => "charui/Core/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/Core/text_energy.png".ImagePath();
}