using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class BlowpipeGrowth() : ExtraSimpleWeaponCard(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new IntVar("Darts", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        for (int i = 0; i < DynamicVars["Darts"].IntValue; i++)
        {
            await BlowpipeDarts.AddDart(CombatState, Owner, PileType.Draw, CardPilePosition.Random);
        }

        // Every play shuffles one more dart for the rest of the combat.
        DynamicVars["Darts"].BaseValue += 1m;
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Dart>())
    ];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
