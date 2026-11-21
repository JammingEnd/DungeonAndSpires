using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotBullseye() : ExtraSimpleWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(10, ValueProp.Move),
        new PowerVar<SlingshotBullseyeDexterityPower>("Dexterity", 2)
    ];

    private bool IsFirstAttackThisTurn =>
        CombatManager.Instance.History.CardPlaysStarted.Count(e => e.HappenedThisTurn(CombatState) && e.CardPlay.Player == Owner && e.CardPlay.Card.Type == CardType.Attack) <= 1;

    protected override bool ShouldGlowGoldInternal => IsFirstAttackThisTurn;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        if (IsFirstAttackThisTurn)
        {
            await PowerCmd.Apply<SlingshotBullseyeDexterityPower>(choiceContext, Owner.Creature, DynamicVars["Dexterity"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
        DynamicVars["Dexterity"].UpgradeValueBy(1m);
    }
}
