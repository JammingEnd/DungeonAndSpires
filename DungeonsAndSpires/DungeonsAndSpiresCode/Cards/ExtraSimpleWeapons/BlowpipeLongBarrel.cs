using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class BlowpipeLongBarrel() : ExtraSimpleWeaponCard(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    private const int BaseDamage = 5;
    private const int DamagePerDart = 5;

    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        // Preview assumes the draw-pile darts are about to be copied into the discard pile.
        ..MakeCalculatedDamage("Damage", BaseDamage,
            (card, _) => DamagePerDart * (DartsIn(card, PileType.Discard) + DartsIn(card, PileType.Draw)), 1, ValueProp.Move)
    ];

    private static int DartsIn(CardModel card, PileType pile) =>
        pile.GetPile(card.Owner).Cards.Count(c => c is Dart);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { } target)
        {
            return;
        }

        // Copy every Dart in the draw pile into the discard pile.
        foreach (var dart in PileType.Draw.GetPile(Owner).Cards.Where(c => c is Dart).ToList())
        {
            var copy = CombatState.CloneCard(dart);
            await CardPileCmd.Add(copy, PileType.Discard);
        }

        decimal damage = BaseDamage + DamagePerDart * DartsIn(this, PileType.Discard);
        await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(target).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
