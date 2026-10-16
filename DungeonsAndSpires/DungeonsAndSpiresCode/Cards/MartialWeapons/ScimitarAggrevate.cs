using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class ScimitarAggrevate() : MartialWeaponCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.Finesse];
    
    private static readonly SpireField<CardModel, int> PlayedOnTurn = new((CardModel _) => -1);

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(4, ValueProp.Move),
        new IntVar("Hits", 2)
    ];

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this)
        {
            PlayedOnTurn[this] = Owner.PlayerCombatState?.TurnNumber ?? -1;
        }
        return Task.CompletedTask;
    }
    
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != this)
        {
            return 1m;
        }

        int played = PlayedOnTurn[this];
        int turn = Owner.PlayerCombatState?.TurnNumber ?? 0;
        return played >= 0 && turn == played + 1 ? 2m : 1m;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, DynamicVars["Hits"].IntValue).Execute(choiceContext);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(2m);
    }
}
