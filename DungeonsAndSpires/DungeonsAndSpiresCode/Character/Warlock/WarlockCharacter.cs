using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Basic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Sorcerer;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Warlock;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Sorcerer;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Warlock;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class WarlockCharacter : PlaceholderCharacterModel
{
    public const string CharacterId = "WarlockCharacter";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 65;

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
        ModelDb.Card<ArmorOfAgathys>(),
        ModelDb.Card<EldritchBlast>(),
        ModelDb.Card<EldritchBlast>()
    ];

    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<GreatOneRelic>()
    ];

    public override CardPoolModel CardPool => ModelDb.CardPool<WarlockCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<WarlockRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<WarlockPotionPool>();
    
    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "Warlock/character_icon_warlock.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "Warlock/char_select_warlock.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "Warlock/char_select_warlock_locked.png".CharacterUiPath();
    public override string CustomCharacterSelectBg => "Warlock/char_select_bg_warlock.tscn".CharacterUiPath();
    public override string CustomMapMarkerPath => "Warlock/map_marker_warlock.png".CharacterUiPath();
    public override string CustomEnergyCounterPath => "res://DungeonsAndSpires/Scenes/Core/DAS_energy_counter.tscn";
    public override string CustomVisualPath => "res://DungeonsAndSpires/Scenes/Warlock/warlock_visuals.tscn";
    public override string? CustomRestSiteAnimPath => "res://DungeonsAndSpires/Scenes/Warlock/warlock_rest_site.tscn";
    public override string CustomMerchantAnimPath => "res://DungeonsAndSpires/Scenes/Warlock/warlock_merchant.tscn";
}
