using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class BlowpipeSticknStones() : ExtraSimpleWeaponCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar(2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-BLOWPIPE_STICKN_STONES.selectionPrompt"), 0, DynamicVars.Cards.IntValue);
        if (BlowpipeDarts.GenerationLocked(Owner))
        {
            return;
        }

        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, null, this);

        foreach (var card in selected)
        {
            var dart = CombatState.CreateCard<Dart>(Owner);
            if (IsUpgraded)
            {
                CardCmd.Upgrade(dart);
            }

            await CardCmd.Transform(card, dart);
        }
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Dart>())
    ];
    protected override void OnUpgrade()
    {
        DynamicVars.Cards.UpgradeValueBy(1);
    }
}
