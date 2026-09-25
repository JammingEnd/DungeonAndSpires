using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class Catapult() : SpellCard(1, 2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Ritual
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var weapons = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Tags.Contains(DASCoreCardtags.Weapon))
            .ToList();
        if (weapons.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-CATAPULT.selectionPrompt"), 1, 1);
        var picked = await CardSelectCmd.FromHand(choiceContext, this.Owner, prefs, model =>
        {
            return model.Tags.Contains(DASCoreCardtags.Weapon);
        }, this);
        var weapon = picked.FirstOrDefault();
        if (weapon == null)
        {
            return;
        }

        weapon.AddKeyword(CoreKeywords.Potent);
        weapon.AddKeyword(CardKeyword.Exhaust);
    }
}