using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Barbarian;

[Pool(typeof(BarbarianRelicPool))]
public class GiantRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    public override RelicModel? GetUpgradeReplacement() => ModelDb.Relic<MiniatureMountainRelic>();
    public override async Task InitialBonus() { }
    public override async Task Act2Bonus() { }
    public override async Task Act3Bonus() { }

    // act1: at the start of your turn, while raged, a random attack in your hand gains a 1 use 3 strength Lightning/thunder dip.
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner || CurrentLevel < 1 || Owner.Creature.GetPower<BarbarianRagePower>() == null)
        {
            return;
        }

        var attacks = Owner.PlayerCombatState?.Hand.Cards.Where(c => c.Type == CardType.Attack && !c.HasDip()).ToList();
        if (attacks == null || attacks.Count == 0)
        {
            return;
        }

        var card = Owner.RunState.Rng.CombatCardSelection.NextItem(attacks);
        var type = Owner.RunState.Rng.CombatCardSelection.NextBool() ? CoreKeywords.Lightning : CoreKeywords.Thunder;
        var dip = DipExtentions.GenerateDip(type, 2, 1);
        await DipCmd.Add(choiceContext, Owner, card, dip);
    }

    // act2: applying reverb or shocked gives you 2 block
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power,
        decimal amount, Creature? applier, CardModel? cardSource)
    {
        if (CurrentLevel < 2 || amount <= 0 || power is not (Reverberation or ShockedPower))
        {
            return;
        }

        bool mine = applier == Owner.Creature
                    || power.Applier == Owner.Creature
                    || (cardSource != null && cardSource.Owner == Owner);

        if (!mine)
        {
            return;
        }

        await CreatureCmd.GainBlock(Owner.Creature, 2, ValueProp.Move, null);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<BarbarianRagePower>()
    ];
}
