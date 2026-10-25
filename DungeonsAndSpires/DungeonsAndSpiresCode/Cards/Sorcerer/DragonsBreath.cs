using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Sorcerer;

public class DragonsBreath() : SpellCard(2, 2, CardType.Attack, CardRarity.Basic, TargetType.AllEnemies), ITranscendenceCard
{
    public override IEnumerable<CardKeyword> ElementOptions =>
    [
        CoreKeywords.Fire,
        CoreKeywords.Cold,
        CoreKeywords.Thunder,
        CoreKeywords.Lightning,
        CoreKeywords.Acid,
        CoreKeywords.Poison
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Unpowered),
        new PowerVar<PoisonPower>("PoisonPower", 2),
        new PowerVar<VulnerablePower>("VulnerablePower", 1),
        new PowerVar<FrostbitePower>("FrostbitePower", 1),
        new PowerVar<Reverberation>("Reverberation", 1),
        new PowerVar<ShockedPower>("ShockedPower", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await ChooseElement(choiceContext);
        foreach (var enemy in CombatState.HittableEnemies!)
        {
            await DamageCmd.Attack(DynamicVars.Damage.IntValue).FromCard(this, cardPlay).Targeting(enemy).Execute(choiceContext);
            await ApplyImbuedElementEffect(choiceContext, enemy);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
        HoverTipFactory.FromPower<FrostbitePower>(),
        HoverTipFactory.FromPower<Reverberation>(),
        HoverTipFactory.FromPower<ShockedPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
        DynamicVars["PoisonPower"].UpgradeValueBy(2m);
        DynamicVars["VulnerablePower"].UpgradeValueBy(1m);
        DynamicVars["FrostbitePower"].UpgradeValueBy(1m);
        DynamicVars["ShockedPower"].UpgradeValueBy(1m);
    }

    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Get<ChaosUltra>();
    }
}