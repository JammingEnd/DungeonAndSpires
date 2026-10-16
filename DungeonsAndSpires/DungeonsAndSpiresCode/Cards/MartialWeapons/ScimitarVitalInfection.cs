using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class ScimitarVitalInfection() : MartialWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.Finesse];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(5, ValueProp.Move),
        new IntVar("Vulnerable", 1),
        new IntVar("Weak", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        if (!this.HasDip())
        {
            return;
        }

        await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, DynamicVars["Vulnerable"].IntValue, Owner.Creature, this);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["Weak"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["Vulnerable"].UpgradeValueBy(1);
        DynamicVars["Weak"].UpgradeValueBy(1);
    }
}
