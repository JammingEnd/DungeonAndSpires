using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

using MegaCrit.Sts2.Core.HoverTips;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class RayOfFrost() : SpellCard(0, 1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Cold
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(6, ValueProp.Unpowered),
        new PowerVar<WeakPower>("WeakPower", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, DynamicVars["WeakPower"].IntValue, Owner.Creature, this);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(1m);
        DynamicVars["WeakPower"].UpgradeValueBy(1m);
    }
}