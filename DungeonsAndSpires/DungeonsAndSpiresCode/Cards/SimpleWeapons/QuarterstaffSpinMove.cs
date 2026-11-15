using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class QuarterstaffSpinMove() : SimpleWeaponCard(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Bludgeoning
    ];

    protected override bool HasEnergyCostX => true;

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(6, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = ResolveEnergyXValue();
        if (x <= 0)
        {
            return;
        }

        await CommonActions.CardAttack(this, cardPlay, x).Execute(choiceContext);

        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-QUARTERSTAFF_SPIN_MOVE.selectionPrompt"), 0, x);
        var discarded = await CardSelectCmd.FromHandForDiscard(choiceContext, Owner, prefs, null, this);

        await CardPileCmd.Draw(choiceContext, discarded.Count(), Owner);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
