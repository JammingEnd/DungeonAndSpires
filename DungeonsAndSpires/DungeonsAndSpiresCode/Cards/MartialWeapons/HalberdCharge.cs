using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class HalberdCharge() : MartialWeaponCard(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.TwoHanded];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<HalberdTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new IntVar("Strength", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).TargetingAllOpponents(CombatState!).Execute(choiceContext);
        await PowerCmd.Apply<HalberdChargeStrengthPower>(choiceContext, Owner.Creature, DynamicVars["Strength"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Strength"].UpgradeValueBy(1);
    }
}
