using BaseLib.Utils;
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

public class HalberdUnyielding() : MartialWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.TwoHanded, CardKeyword.Exhaust];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<HalberdTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Move),
        new IntVar("Vigor", 5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await PowerCmd.Apply<HalberdUnyieldingPower>(choiceContext, Owner.Creature, DynamicVars["Vigor"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Vigor"].UpgradeValueBy(2);
    }
}
