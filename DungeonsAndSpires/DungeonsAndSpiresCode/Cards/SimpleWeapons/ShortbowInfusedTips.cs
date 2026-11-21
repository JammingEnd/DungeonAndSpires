using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class ShortbowInfusedTips() : SimpleWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    protected override HashSet<CardTag> CanonicalTags =>
    [
        ..base.CanonicalTags,
        DASCoreCardtags.Bow
    ];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Piering
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        var dip = Owner.PlayerCombatState?.GetDip(this);
        if (dip == null)
        {
            return;
        }

        await DipEffects.Apply(choiceContext, Owner, this, cardPlay.Target, this);

        int strength = WeaponDipsHandler.EffectiveStrength(this, dip, Owner.Creature);
        if (strength > 0)
        {
            await PowerCmd.Apply<ShortbowInfusedTipsDexterityPower>(choiceContext, Owner.Creature, strength, Owner.Creature, this);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip,
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}
