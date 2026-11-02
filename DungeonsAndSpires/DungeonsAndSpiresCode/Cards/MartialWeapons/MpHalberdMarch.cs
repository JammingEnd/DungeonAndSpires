using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

// Choose a different player, whenever you play a two handed weapon this turn, they gain 1 energy
public class MpHalberdMarch() : MartialWeaponCard(0, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.TwoHanded, CardKeyword.Retain, CardKeyword.Exhaust];
    public override TwoHandedPower? TwoHandedPower => ModelDb.Get<HalberdTwoHandedPower>();

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<HalberdMarchPower>(choiceContext, cardPlay.Target, 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        this.RemoveKeyword(CardKeyword.Exhaust);
    }
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

}