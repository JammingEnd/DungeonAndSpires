using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class LongswordAlchemy() : MartialWeaponCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing];

    // The dip types the player can choose from.
    public override IEnumerable<CardKeyword> ElementOptions =>
    [
        CoreKeywords.Fire,
        CoreKeywords.Cold,
        CoreKeywords.Thunder,
        CoreKeywords.Lightning,
        CoreKeywords.Poison
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(6, ValueProp.Move),
        new IntVar("DipTurns", 2),
        new IntVar("DipStrength", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        var drawPile = PileType.Draw.GetPile(Owner);
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-LONGSWORD_ALCHEMY.selectionPrompt"), 1, 1);
        Func<CardModel, bool> filter = c => c.Tags.Contains(DASCoreCardtags.Weapon) && c.Type == CardType.Attack && !c.HasDip();
        var selected = await CardSelectCmd.FromCombatPile(choiceContext, drawPile, Owner, prefs, filter);
        var card = selected.FirstOrDefault();
        if (card == null)
        {
            return;
        }

        var type = await SelectElement(choiceContext);
        if (type is not { } dipType)
        {
            return;
        }

        await DipCmd.Add(choiceContext, Owner, card, new DipInfo(dipType, DynamicVars["DipStrength"].IntValue, DynamicVars["DipTurns"].IntValue));
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
