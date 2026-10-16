using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class LongswordDisposable() : MartialWeaponCard(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var weapons = PileType.Draw.GetPile(Owner).Cards
            .Where(c => c.Tags.Contains(DASCoreCardtags.Weapon))
            .ToList();
        if (weapons.Count == 0)
        {
            return;
        }

        var random = Owner.RunState.Rng.CombatCardSelection.NextItem<CardModel>(weapons);
        random?.EnergyCost.AddThisCombat(-1);
        CardCmd.Preview(random);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
