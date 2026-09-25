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
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class Resistance() : SpellCard(0, 1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Block", (1, 1) }
    };

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(7, ValueProp.Unpowered)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal block = DynamicVars["Block"].GetCalculatedValue();
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Unpowered, cardPlay);

        decimal allyBlock = Math.Floor(block / 2);
        foreach (var ally in CombatState!.Allies)
        {
            if (ally == Owner.Creature)
            {
                continue;
            }
            await CreatureCmd.GainBlock(ally, allyBlock, ValueProp.Unpowered, cardPlay);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["BlockBase"].UpgradeValueBy(3m);
    }
}