using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using DungeonsAndSpires.DungeonsAndSpiresCode.WeaponPlays;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class HalberdStance() : MartialWeaponCard(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<HalberdTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Dexterity", 1),
        ..MakeCalculatedVar("Gain", 0, (card, target) =>
        {
            int attacks = card.Owner?.PlayerCombatState?.GetWeaponPlay()?.WeaponAttacks ?? 0;
            int perAttack = card.DynamicVars.TryGetValue("Dexterity", out var dexterity) ? dexterity.IntValue : 0;
            return attacks * perAttack;
        }, 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int gained = (int)DynamicVars["Gain"].GetCalculatedValue();
        if (gained <= 0)
        {
            return;
        }

        await PowerCmd.Apply<DexterityPower>(choiceContext, Owner.Creature, gained, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["Dexterity"].UpgradeValueBy(1);
    }
}
