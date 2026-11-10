using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class SlingshotGoldBall() : ExtraSimpleWeaponCard(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Ethereal];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var pile = PileType.Draw.GetPile(Owner).Cards;
        if (pile.Count == 0)
        {
            return;
        }

        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(pile);
        if (!card.IsUpgraded)
        {
            CardCmd.Upgrade(card);
        }

        await CardCmd.AutoPlay(choiceContext, card, null);
    }

    protected override void OnUpgrade()
    {
        RemoveKeyword(CardKeyword.Ethereal);
    }
}
