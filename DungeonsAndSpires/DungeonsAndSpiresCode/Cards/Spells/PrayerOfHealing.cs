using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class PrayerOfHealing() : SpellCard(2, 2, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Strength", (3, 1) },
        { "Dexterity", (3, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<TemporaryStrengthPower>("Strength", 1),
        new PowerVar<TemporaryDexterityPower>("Dexterity", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal strength = DynamicVars["Strength"].GetCalculatedValue();
        decimal dexterity = DynamicVars["Dexterity"].GetCalculatedValue();

        var power = await PowerCmd.Apply<PrayerOfHealingPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        power?.SetAmounts(strength, dexterity);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["StrengthBase"].UpgradeValueBy(1m);
        DynamicVars["StrengthExtra"].UpgradeValueBy(1m);
        DynamicVars["DexterityBase"].UpgradeValueBy(1m);
        DynamicVars["DexterityExtra"].UpgradeValueBy(1m);
    }
}