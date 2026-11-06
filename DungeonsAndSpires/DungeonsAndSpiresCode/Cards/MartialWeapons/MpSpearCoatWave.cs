using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

//every attack in all allies hands gain a 2 use 2 strength dip, when an ally attacks with a dip card, gain 2 block
public class MpSpearCoatWave() : MartialWeaponCard(2, CardType.Skill, CardRarity.Rare, TargetType.AllAllies)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.TwoHanded];
    public override TwoHandedPower? TwoHandedPower => ModelDb.Get<SpearTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new IntVar("dipStrength", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPclay)
    {
        await PowerCmd.Apply<SpearCoatWavePower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
        
        foreach (var inst in CombatState.PlayerCreatures.Where(x => x != Owner.Creature) )
        {
            Player ally = inst.Player;
            var handPile = PileType.Hand.GetPile(ally);
            foreach (var card in handPile.Cards.Where(c => c.Type == CardType.Attack))
            {
                await DipCmd.Add(choiceContext, ally, card, DipExtentions.GenerateRandomDip(2, ally, DynamicVars["dipStrength"].IntValue));
            }
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars["dipStrength"].UpgradeValueBy(1);
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        DipTips.Dip
    ];
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

}