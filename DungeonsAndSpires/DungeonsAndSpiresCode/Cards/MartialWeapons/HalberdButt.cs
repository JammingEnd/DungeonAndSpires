using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class HalberdButt() : MartialWeaponCard(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.TwoHanded];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<HalberdTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(6, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        var hand = Owner.PlayerCombatState?.Hand.Cards;
        if (hand == null)
        {
            return;
        }

        var candidates = hand.Where(c =>
            c.Tags.Contains(DASCoreCardtags.Weapon) &&
            c.Type == CardType.Attack &&
            !c.Keywords.Contains(CoreKeywords.TwoHanded) &&
            c.EnergyCost.GetResolved() == 1).ToList();
        if (candidates.Count == 0)
        {
            return;
        }

        var pick = Owner.RunState.Rng.CombatCardSelection.NextItem<CardModel>(candidates);
        pick?.EnergyCost.SetThisTurn(0);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
