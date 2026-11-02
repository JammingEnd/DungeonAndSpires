using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// this turn, when an ally plays a skill, gain 2 vigor
public class MpGreatswordStar() : MartialWeaponCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.TwoHanded, CardKeyword.Retain, CardKeyword.Exhaust];
    public override TwoHandedPower? TwoHandedPower => ModelDb.Get<GreatswordTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<VigorPower>("vigor", 2)];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<GreatswordStarPower>(choiceContext, Owner.Creature, DynamicVars["vigor"].IntValue,
            Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["vigor"].UpgradeValueBy(1);
    }

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;
}