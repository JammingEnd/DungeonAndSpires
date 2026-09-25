using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;

// Choice-token cards used by Chromatic Orb's element selection screen. They are never played
// or generated in combat; they only represent the selectable element.
[Pool(typeof(DASSpellCardPool))]
public abstract class ElementChoiceCard : CoreCard
{
    protected ElementChoiceCard() : base(0, CardType.Status, CardRarity.Token, TargetType.None)
    {
    }

    public override bool CanBeGeneratedInCombat => false;
}

public class ElementFire : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Fire];
}

public class ElementIce : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Cold];
}

public class ElementLightning : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Lightning];
}

public class ElementThunder : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Thunder];
}

public class ElementAcid : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Acid];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<VulnerablePower>("VulnerablePower", 1)
    ];
}

public class ElementPoison : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Poison];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<PoisonPower>("PoisonPower", 2)
    ];
}