using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Basic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class RogueCharacter : PlaceholderCharacterModel
{
    public const string CharacterId = "RogueCharacter";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 75;

    // Copied from the Fighter; swap in Rogue cards once they exist.
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<SpearJavalin>(),
        ModelDb.Card<GreatswordMaverick>(),
        ModelDb.Card<GreatswordExpertise>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BattleMasterRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<RogueCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<RogueRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<RoguePotionPool>();

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    // Everything else uses PlaceholderCharacterModel's base game assets until Rogue art exists:
    // "Rogue/character_icon_rogue.png", "Rogue/char_select_bg_rogue.tscn", Scenes/Rogue/rogue_visuals.tscn, ...
    public override string CustomIconTexturePath => "Rogue/character_icon_rogue.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "Rogue/char_select_rogue.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "Rogue/char_select_rogue_locked.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "Rogue/char_select_bg_rogue.tscn".CharacterUiPath();
    public override string CustomMapMarkerPath => "Rogue/map_marker_rogue.png".CharacterUiPath();
    public override string CustomEnergyCounterPath => "res://DungeonsAndSpires/Scenes/Core/DAS_energy_counter.tscn";
    public override string CustomVisualPath => "res://DungeonsAndSpires/Scenes/rogue/rogue_visuals.tscn";
    public override string? CustomRestSiteAnimPath => "res://DungeonsAndSpires/Scenes/rogue/rogue_rest_site.tscn";
}
