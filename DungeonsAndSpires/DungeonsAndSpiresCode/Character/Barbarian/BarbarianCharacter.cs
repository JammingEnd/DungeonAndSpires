using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Basic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Rogue;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class BarbarianCharacter : PlaceholderCharacterModel
{
    public const string CharacterId = "BarbarianCharacter";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 80;

    // Copied from the Rogue; swap in Barbarian cards once they exist.
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<DaggerReliable>(),
        ModelDb.Card<DaggerThroat>(),
    ];

    // Placeholder until Barbarian subclass relics exist.
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<AssassinRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<BarbarianCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BarbarianRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BarbarianPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "Barbarian/character_icon_barbarian.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "Barbarian/char_select_barbarian.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "Barbarian/char_select_barbarian_locked.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "Barbarian/char_select_bg_barbarian.tscn".CharacterUiPath();
    public override string CustomMapMarkerPath => "Barbarian/map_marker_barbarian.png".CharacterUiPath();
    public override string CustomEnergyCounterPath => "res://DungeonsAndSpires/Scenes/Core/DAS_energy_counter.tscn";
    public override string CustomVisualPath => "res://DungeonsAndSpires/Scenes/barbarian/barbarian_visuals.tscn";
    public override string? CustomRestSiteAnimPath => "res://DungeonsAndSpires/Scenes/barbarian/barbarian_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://DungeonsAndSpires/Scenes/barbarian/barbarian_merchant.tscn";
}
