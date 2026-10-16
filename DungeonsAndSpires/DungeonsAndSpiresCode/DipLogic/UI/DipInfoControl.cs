using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Nodes;

public partial class DipInfoControl : Control
{
    private NCard? _view;

    public void Initialize(NCard view)
    {
        _view = view;
        Visible = false;
        DipCmd.OnDipChanged += OnDipChanged;
        Refresh();
    }

    private void OnDipChanged(CardModel card)
    {
        if (!GodotObject.IsInstanceValid(this))
        {
            DipCmd.OnDipChanged -= OnDipChanged;
            return;
        }

        if (card == _view?.Model)
        {
            Refresh();
        }
    }

    public void Refresh()
    {
        var card = _view?.Model;
        // Canonical models (e.g. transform previews) can't be inspected in combat; hide the overlay.
        if (card == null || !card.IsMutable)
        {
            Visible = false;
            return;
        }

        if (!card.Tags.Contains(DASCoreCardtags.Weapon))
        {
            Visible = false;
            return;
        }

        var dip = card.Owner?.PlayerCombatState?.GetDip(card);
        Visible = dip != null;
        if (dip == null)
        {
            return;
        }

        GetNodeOrNull<Label>("UseLabel")?.SetText(dip.UsesLeft.ToString());
        // Match WeaponDipsHandler's damage formula: dip strength * this card's multiplier + Homebrew.
        int multiplier = card is CoreCard core ? core.DipStrengthMultiplier : 1;
        int homebrew = card.Owner?.Creature?.GetPowerAmount<ScimitarHomebrewPower>() ?? 0;
        GetNodeOrNull<Label>("StrengthLabel")?.SetText((dip.Strength * multiplier + homebrew).ToString());

        var typeRect = GetNodeOrNull<TextureRect>("TypeRect");
        if (typeRect != null)
        {
            var icon = $"{MainFile.ResPath}/images/cards/dip_icons/{DipTypeIcon(dip.Type)}.png";
            var texture = GD.Load<Texture2D>(icon);
            if (texture != null)
            {
                typeRect.Texture = texture;
            }
        }
    }

    private static string DipTypeIcon(CardKeyword type)
    {
        if (type == CoreKeywords.Cold)
            return "cold";
        if (type == CoreKeywords.Fire)
            return "flamer";
        if (type == CoreKeywords.Lightning)
            return "lightning";
        if (type == CoreKeywords.Poison)
            return "poison";
        if (type == CoreKeywords.Thunder)
            return "thunder";
        if (type == CoreKeywords.Acid)
            return "acid";
        return "lightning";
    }
}