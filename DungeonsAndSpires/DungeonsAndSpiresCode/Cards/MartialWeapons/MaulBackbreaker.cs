using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class MaulBackbreaker() : MartialWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Bludgeoning, CoreKeywords.TwoHanded];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<MaulTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(9, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        var dip = Owner.PlayerCombatState?.GetDip(this);
        var target = cardPlay.Target;
        if (dip == null || target == null)
        {
            return;
        }

        int amount = WeaponDipsHandler.EffectiveStrength(this, dip, Owner.Creature);
        if (amount > 0)
        {
            if (dip.Type == CoreKeywords.Poison)
            {
                await PowerCmd.Apply<PoisonPower>(choiceContext, target, amount, Owner.Creature, this);
            }
            else if (dip.Type == CoreKeywords.Cold)
            {
                await PowerCmd.Apply<FrostbitePower>(choiceContext, target, amount, Owner.Creature, this);
            }
            else if (dip.Type == CoreKeywords.Lightning)
            {
                await PowerCmd.Apply<ShockedPower>(choiceContext, target, amount, Owner.Creature, this);
            }
            else if (dip.Type == CoreKeywords.Thunder && target.GetPowerAmount<Reverberation>() == 0)
            {
                await PowerCmd.Apply<Reverberation>(choiceContext, target, 1, Owner.Creature, this);
            }
        }

        await DipCmd.Remove(choiceContext, Owner, this, dip);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(5m);
    }
}
