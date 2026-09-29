using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class ShortswordCursed() : SimpleWeaponCard(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Slashing
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Move)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        IEnumerable<CardModel>[] piles = [
            PileType.Hand.GetPile(Owner).Cards,
            PileType.Draw.GetPile(Owner).Cards,
            PileType.Discard.GetPile(Owner).Cards
        ];
        var statuses = piles.SelectMany(p => p)
            .Where(c => c.Type == CardType.Status)
            .ToList();
        if (statuses.Count == 0)
        {
            return;
        }

        var status = statuses[Owner.RunState.Rng.CombatCardSelection.NextInt(statuses.Count)];

        var colorless = ModelDb.CardPool<ColorlessCardPool>().AllCards.ToList();
        if (colorless.Count == 0)
        {
            return;
        }
        var newCard = colorless[Owner.RunState.Rng.CombatCardSelection.NextInt(colorless.Count)];
        var card = CombatState.CreateCard(newCard, Owner);
        await CardCmd.Transform(status, card);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(3m);
    }
}