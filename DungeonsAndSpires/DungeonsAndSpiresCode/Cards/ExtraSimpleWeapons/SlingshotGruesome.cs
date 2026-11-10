using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotGruesome() : ExtraSimpleWeaponCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(18, ValueProp.Move),
        new IntVar("Attacks", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        // when gruesome applies the initial power, it should not trigger it. 
        bool alreadyActive = cardPlay.Target.Powers.OfType<SlingshotGruesomePower>().Any(p => p.Applier == Owner.Creature);

        var power = await PowerCmd.Apply<SlingshotGruesomePower>(choiceContext, cardPlay.Target, DynamicVars["Attacks"].IntValue, Owner.Creature, this);
        power?.SetSource(alreadyActive ? null : this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Attacks"].UpgradeValueBy(2);
    }
}
