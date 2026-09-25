using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class MoldEarth() : SpellCard(1, 1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Block", (3, 2) },
        { "Weak", (3, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(4, ValueProp.Unpowered),
        new IntVar("Weak", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars["Block"].GetCalculatedValue(), ValueProp.Unpowered, cardPlay);

        var enemies = CombatState!.HittableEnemies.ToArray();
        await PowerCmd.Apply<WeakPower>(choiceContext, enemies, DynamicVars["Weak"].GetCalculatedValue(), Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["BlockBase"].UpgradeValueBy(2m);
        DynamicVars["BlockExtra"].UpgradeValueBy(1m);
    }
}