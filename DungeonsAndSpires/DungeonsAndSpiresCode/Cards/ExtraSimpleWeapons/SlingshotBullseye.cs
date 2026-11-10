using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotBullseye() : ExtraSimpleWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new IntVar("Bonus", 10)
    ];

    protected override bool ShouldGlowGoldInternal => CombatManager.Instance.History.CardPlaysStarted.Count(e => e.HappenedThisTurn(CombatState) && e.CardPlay.Player == Owner) <= 1;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool first = CombatManager.Instance.History.CardPlaysStarted.Count(e => e.HappenedThisTurn(CombatState) && e.CardPlay.Player == Owner) <= 1;

        var damage = DynamicVars.Damage.BaseValue + (first ? DynamicVars["Bonus"].IntValue : 0m);
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Bonus"].UpgradeValueBy(5);
    }
}
