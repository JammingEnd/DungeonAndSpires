using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class Disintegrate() : SpellCard(6, 3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Damage", (1, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(23, ValueProp.Unpowered),
        new CardsVar("Exhaust", 3)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, DynamicVars["Damage"].GetCalculatedValue(), ValueProp.Unpowered, Owner.Creature, this, cardPlay);

        var drawPile = PileType.Draw.GetPile(Owner);
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-DISINTEGRATE.selectionPrompt"), 0, DynamicVars["Exhaust"].IntValue);
        Func<CardModel, bool> filter = c => c is not SpellCard;
        var selected = await CardSelectCmd.FromCombatPile(choiceContext, drawPile, Owner, prefs, filter);
        foreach (var card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DamageBase"].UpgradeValueBy(7m);
    }
}