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
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotCapsule() : ExtraSimpleWeaponCard(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        ..MakeCalculatedDamage("Damage", 1, (card, _) => TotalDipStrength(card), 1, ValueProp.Move)
    ];

    private static int TotalDipStrength(CardModel card)
    {
        var state = card.Owner.PlayerCombatState?.GetDipState();
        if (state == null)
        {
            return 0;
        }

        return state.WeaponDips.Sum(dip => WeaponDipsHandler.EffectiveStrength(dip.Key, dip.Value, card.Owner.Creature));
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
       
        decimal damage = DynamicVars["Damage"].GetCalculatedValue();

        var state = Owner.PlayerCombatState?.GetDipState();
        if (state != null)
        {
            foreach (var dip in state.WeaponDips.ToList())
            {
                await DipCmd.Remove(choiceContext, Owner, dip.Key, dip.Value);
            }
        }

        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target).Execute(choiceContext);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip
    ];

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
