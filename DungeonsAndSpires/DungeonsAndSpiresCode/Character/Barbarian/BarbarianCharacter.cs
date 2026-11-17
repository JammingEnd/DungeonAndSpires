using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Barbarian;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Basic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Rogue;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;
using DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Rogue;
using DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Character;

public class BarbarianCharacter : PlaceholderCharacterModel, IAbilityCast
{
    public const string CharacterId = "BarbarianCharacter";

    public static readonly Color Color = new("ffffff");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;
    public override int StartingHp => 90;
    
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
        ModelDb.Card<MaulFullSwing>(),
        ModelDb.Card<MaulKnuckle>(),
    ];
    
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BeserkerRelic>()
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


    public string AbilityIconPath => "res://DungeonsAndSpires/images/powers/barbarian_rage_power.png";

    public int MaxCharges => 2;

    public int GetCharges(Player player) => AbilityTracker.GetCharges(player);

    public void CastAbility(Player player)
    {
        if(player.Creature.GetPower<BarbarianRagePower>() != null)
            return;
        if (AbilityTracker.SpendCharge(player))
        {
            TaskHelper.RunSafely(ApplyRage(player));
        }
    }

    public LocString AbilityTitle => new("static_hover_tips", "DUNGEONSANDSPIRES-RAGE_ABILITY.title");
    public LocString AbilityDescription => new("static_hover_tips", "DUNGEONSANDSPIRES-RAGE_ABILITY.description");

    private async Task ApplyRage(Player player)
    {
        await PowerCmd.Apply<BarbarianRagePower>(new ThrowingPlayerChoiceContext(), player.Creature, 1, player.Creature, null);
    }
}
