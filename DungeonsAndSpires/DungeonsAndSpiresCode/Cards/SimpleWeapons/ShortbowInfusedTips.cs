using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
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
        new DamageVar(7, ValueProp.Move),
        new PowerVar<PoisonPower>("PoisonPower", 2),
        new PowerVar<VulnerablePower>("VulnerablePower", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var element = ImbuedElement;

        decimal damage = DynamicVars["Damage"].GetCalculatedValue();
        if (element != null && element != CoreKeywords.Poison && element != CoreKeywords.Acid)
        {
            damage += 4;
        }
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, damage, ValueProp.Move, Owner.Creature, this, cardPlay);
        
        if (element == CoreKeywords.Poison)
        {
            await PowerCmd.Apply<PoisonPower>(choiceContext, cardPlay.Target, GetElementEffectAmount(element.Value) * 2, Owner.Creature, this);
        }
        else if (element == CoreKeywords.Acid)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, cardPlay.Target, GetElementEffectAmount(element.Value) * 2, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        this.AddKeyword(CardKeyword.Retain);
    }
}