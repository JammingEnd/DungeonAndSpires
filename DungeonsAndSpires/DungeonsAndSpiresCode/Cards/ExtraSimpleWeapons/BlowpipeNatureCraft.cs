using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class BlowpipeNatureCraft() : ExtraSimpleWeaponCard(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int x = ResolveEnergyXValue() + (IsUpgraded ? 1 : 0);
        if (x <= 0)
        {
            return;
        }

        for (int i = 0; i < x; i++)
        {
            var dart = CombatState.CreateCard<Dart>(Owner);
            await CardPileCmd.AddGeneratedCardToCombat(dart, PileType.Draw, Owner, CardPilePosition.Top);
        }

        await PowerCmd.Apply<BlowpipeNatureCraftPower>(choiceContext, Owner.Creature, x, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromCard(ModelDb.Card<Dart>())
    ];

    protected override void OnUpgrade()
    {
    }
}
