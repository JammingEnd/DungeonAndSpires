using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.CombatHistoryEntries;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;

public class MaceParallel() : ExtraSimpleWeaponCard(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(11, ValueProp.Move)
    ];

    protected override bool ShouldGlowGoldInternal => CombatManager.Instance.History.Entries
        .OfType<ResonanceActivationEntry>().Any(e => e.Actor == Owner.Creature && e.HappenedThisTurn(CombatState));

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        bool activated = CombatManager.Instance.History.Entries.OfType<ResonanceActivationEntry>().Any(e => e.Actor == Owner.Creature && e.HappenedThisTurn(CombatState));
        decimal block = DynamicVars.Block.BaseValue * (activated ? 2m : 1m);
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        AddKeyword(CardKeyword.Innate);
    }
    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<ResonancePower>(),
    ];
}
