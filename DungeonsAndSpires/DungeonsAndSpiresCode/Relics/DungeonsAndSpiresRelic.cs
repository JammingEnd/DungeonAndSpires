using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using System.Text.RegularExpressions;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics;

/// <summary>
/// This is the base class for your mod's relics, which is set up to load the relic's images from your mod's resources.
/// When creating a relic, right click the Relics folder and create a new file with the Custom Relic template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
///
/// The [Pool] annotation marks this relic as being tied to your specific character. Inheriting from this class means
/// that your relics don't need to invidually say which pool they should be in.
/// </summary>
[Pool(typeof(SorcererRelicPool))]
public abstract class DungeonsAndSpiresRelic : CustomRelicModel
{
    private static readonly Regex SnakeRegex = new("([a-z0-9])([A-Z])");

    private string RelicName() => SnakeRegex.Replace(GetType().Name, "$1_$2").ToLowerInvariant();

    public override string PackedIconPath => $"{RelicName()}.png".RelicImagePath();

    protected override string PackedIconOutlinePath => $"{RelicName()}_outline.png".RelicImagePath();

    protected override string BigIconPath => $"{RelicName()}.png".BigRelicImagePath();
}