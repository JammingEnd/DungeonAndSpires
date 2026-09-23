using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using Godot;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class SorcererRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => SorcererCharacter.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}