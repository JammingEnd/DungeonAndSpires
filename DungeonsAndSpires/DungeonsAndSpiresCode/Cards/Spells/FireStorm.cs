using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.CombatHistoryEntries;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class FireStorm() : SpellCard(6, 1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CardVars =>
        MakeCalculatedVar("CurrentDamage", 0, (card, target) => CombatManager.Instance.History.Entries.OfType<FireDamageEntry>().Count(), 3);

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal damage = DynamicVars["CurrentDamage"].GetCalculatedValue(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, damage, ValueProp.Unpowered, Owner.Creature, this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["CurrentDamageExtra"].UpgradeValueBy(1m);
    }
}