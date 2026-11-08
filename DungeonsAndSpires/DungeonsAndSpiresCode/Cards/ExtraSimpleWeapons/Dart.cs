using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class Dart() : ExtraSimpleWeaponCard(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering, CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(2, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Armor Piercing makes darts ignore Block while the buff is up.
        DynamicVars.Damage.Props = Owner.Creature.GetPower<BlowpipeArmorPiercingPower>() != null
            ? ValueProp.Move | ValueProp.Unblockable
            : ValueProp.Move;

        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await CardPileCmd.Draw(choiceContext, 1, Owner);
    }
    
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        decimal bonus = base.ModifyDamageAdditive(target, amount, props, dealer, cardSource, cardPlay);
        if (cardSource != this || dealer != Owner.Creature || target == dealer)
        {
            return bonus;
        }

        decimal strength = dealer.GetPowerAmount<StrengthPower>();
        decimal vigor = dealer.GetPowerAmount<VigorPower>();
        return bonus - (strength + vigor) / 2m;
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
    }
}
