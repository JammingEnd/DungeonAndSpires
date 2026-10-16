using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class ScimitarRedirect() : MartialWeaponCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(6, ValueProp.Move)
    ];
    
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int strength = Owner.Creature.GetPowerAmount<StrengthPower>();
        int dexterity = Owner.Creature.GetPowerAmount<DexterityPower>();
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + Math.Max(strength, dexterity), ValueProp.Move, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
    }
}
