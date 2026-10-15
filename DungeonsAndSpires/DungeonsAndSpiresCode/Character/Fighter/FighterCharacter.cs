using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Basic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Fighter;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Sorcerer;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class FighterCharacter : PlaceholderCharacterModel
{
    public const string CharacterId = "FighterCharacter";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Masculine;
    public override int StartingHp => 75;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubStrike>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<ClubDefend>(),
        ModelDb.Card<Resistance>(),
        ModelDb.Card<Firebolt>(),
        ModelDb.Card<ChromaticOrb>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BattleMasterRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<FighterCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<FighterRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<FighterPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "Fighter/character_icon_fighter.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "Fighter/char_select_fighter.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "Fighter/char_select_fighter_locked.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "Fighter/char_select_bg_fighter.tscn".CharacterUiPath();
    public override string CustomMapMarkerPath => "Fighter/map_marker_fighter.png".CharacterUiPath();
    public override string CustomEnergyCounterPath => "res://DungeonsAndSpires/Scenes/Core/DAS_energy_counter.tscn";
    public override string CustomVisualPath => "res://DungeonsAndSpires/Scenes/Fighter/fighter_visuals.tscn";
    public override string? CustomRestSiteAnimPath => "res://DungeonsAndSpires/Scenes/Fighter/fighter_rest_site.tscn";
}
