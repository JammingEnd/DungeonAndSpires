using MegaCrit.Sts2.Core.Commands;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class CrossbowVolley() : MartialWeaponCard(3, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Piering, CardKeyword.Exhaust];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var hand = Owner.PlayerCombatState?.Hand.Cards.ToList() ?? [];

        foreach (var card in hand.Where(c => c.Type == CardType.Attack).ToList())
        {
            await CardCmd.AutoPlay(choiceContext, card, null);
        }

        foreach (var card in Owner.PlayerCombatState?.Hand.Cards.ToList() ?? [])
        {
            await CardCmd.Exhaust(choiceContext, card);
        }
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Retain);
    }
}
