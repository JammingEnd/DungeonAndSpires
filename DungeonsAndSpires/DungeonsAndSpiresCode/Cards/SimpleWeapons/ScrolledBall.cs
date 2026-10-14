using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;

public class ScrolledBall() : SimpleWeaponCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var spells = ModelDb.CardPool<DASSpellCardPool>().AllCards
            .OfType<SpellCard>()
            .Where(s => s.Level <= 5)
            .ToList();
        if (spells.Count == 0)
        {
            return;
        }

        var canonical = Owner.RunState.Rng.CombatCardGeneration.NextItem(spells);
        if (canonical == null)
        {
            return;
        }

        var spell = CombatState.CreateCard(canonical, Owner);
        spell.AddKeyword(CoreKeywords.Scrolled);
        await CardPileCmd.AddGeneratedCardToCombat(spell, PileType.Hand, Owner);
    }
    
    protected override CardLocation GetResultLocationForCardPlay()
    {
        var location = base.GetResultLocationForCardPlay();
        if (CombatState == null)
        {
            return location;
        }

        var teammates = CombatState.GetTeammatesOf(Owner.Creature)
            .Where(c => c.IsAlive && c.IsPlayer && c.Player != Owner)
            .ToList();
        if (teammates.Count == 0)
        {
            return location;
        }

        location.player = Owner.RunState.Rng.CombatTargets.NextItem(teammates)!.Player;
        if (location.pileType == PileType.Discard)
        {
            location.pileType = PileType.Draw;
            location.position = CardPilePosition.Random;
        }
        return location;
    }
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Scrolled)
    ];
}