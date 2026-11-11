using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class BlowpipeScrap() : ExtraSimpleWeaponCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var scrappie = CombatState.CreateCard(ModelDb.Card<Scrappie>(), cardPlay.Target.Player);
        if (IsUpgraded)
        {
            CardCmd.Upgrade(scrappie);
        }

        Scrappie.SetGiver(scrappie, Owner);
        await CardPileCmd.AddGeneratedCardToCombat(scrappie, PileType.Hand, Owner);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard<Scrappie>(IsUpgraded)
    ];
}
