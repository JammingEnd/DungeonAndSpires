using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class ShockingGrasp() : SpellCard(0, 1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Lightning
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(5, ValueProp.Unpowered),
        new PowerVar<ShockedPower>("Shocked", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);
        await PowerCmd.Apply<ShockedPower>(choiceContext, cardPlay.Target, DynamicVars["Shocked"].IntValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
        DynamicVars["Shocked"].UpgradeValueBy(2m);
    }
}