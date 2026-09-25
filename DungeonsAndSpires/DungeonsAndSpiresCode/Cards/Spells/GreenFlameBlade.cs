using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class GreenFlameBlade() : SpellCard(0, 1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    
    protected override HashSet<CardTag> CanonicalTags => [DASCoreCardtags.Cantrip];

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CoreKeywords.Potent
    ];

    // to use Ability 
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(5, ValueProp.Unpowered),
        ..MakeCalculatedVar("Potency", 0, static (model, creature) =>
        {
            return model.Owner.PlayerCombatState.GetPotency(); 
            // this uses the whole potency. some card may want to have bigger steps, then we'd want to divide by the step size and round down to the nearest int
        }, 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(choiceContext);

        var otherEnemies = CombatState!.HittableEnemies.Where(e => e != cardPlay.Target).ToArray();
        if (otherEnemies.Length == 0)
            return;

        Creature enemy = Owner.RunState.Rng.CombatTargets.NextItem<Creature>(otherEnemies);
        if (enemy == null)
            return;
        int potency = (int)DynamicVars["Potency"].GetCalculatedValue();
        await CreatureCmd.Damage(choiceContext, enemy, potency, ValueProp.Unpowered, Owner.Creature);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}