using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;

// Choice-token cards used by Chromatic Orb's element selection screen. They are never played
// or generated in combat; they only represent the selectable element.
[Pool(typeof(TokenCardPool))]
public abstract class ElementChoiceCard : CoreCard
{
    protected ElementChoiceCard() : base(-1, CardType.None, CardRarity.Token, TargetType.None)
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
    
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<VulnerablePower>("FrostbitePower", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<FrostbitePower>()
    ];
}

public class ElementLightning : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Lightning];
    
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<VulnerablePower>("ShockedPower", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ShockedPower>()
    ];
}

public class ElementThunder : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Thunder];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<VulnerablePower>("Reverberation", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<Reverberation>()
    ];
}

public class ElementAcid : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Acid];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<VulnerablePower>("VulnerablePower", 1)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<VulnerablePower>()
    ];
}

public class ElementPoison : ElementChoiceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Poison];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new PowerVar<PoisonPower>("PoisonPower", 2)
    ];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<PoisonPower>()
    ];
}