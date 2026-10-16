using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class HalberdFlawlessStrike() : MartialWeaponCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.TwoHanded];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<HalberdTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(15, ValueProp.Move),
        ..MakeCalculatedVar("Gain", 0, (card, target) =>
            card.Owner?.PlayerCombatState?.GetWeaponPlay()?.TwoHanded ?? 0, 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        int gained = (int)DynamicVars["Gain"].GetCalculatedValue();
        if (gained > 0)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, Owner.Creature, gained, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
        DynamicVars.Damage.UpgradeValueBy(6m);
    }
}
