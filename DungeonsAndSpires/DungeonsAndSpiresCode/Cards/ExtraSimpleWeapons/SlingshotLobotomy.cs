using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotLobotomy() : ExtraSimpleWeaponCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(15, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        var dip = Owner.PlayerCombatState?.GetDip(this);
        if (dip == null)
        {
            return;
        }

        int loss = 2 * WeaponDipsHandler.EffectiveStrength(this, dip, Owner.Creature);
        if (loss > 0)
        {
            await PowerCmd.Apply<StrengthPower>(choiceContext, cardPlay.Target, -loss, Owner.Creature, this);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip,
        HoverTipFactory.FromPower<StrengthPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(7m);
    }
}
