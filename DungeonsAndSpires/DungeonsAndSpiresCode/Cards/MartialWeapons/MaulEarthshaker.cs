using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class MaulEarthshaker() : MartialWeaponCard(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.TwoHanded];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<MaulTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Damage", 5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MaulEarthshakerPower>(choiceContext, Owner.Creature, DynamicVars["Damage"].IntValue, Owner.Creature, this);
    }
    

    protected override void OnUpgrade()
    {
        DynamicVars["Damage"].UpgradeValueBy(2);
    }
}
