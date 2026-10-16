using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.MartialWeapons;

public class ScimitarQuickSlash() : MartialWeaponCard(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.Slashing, CoreKeywords.Finesse];
    
    public bool DipUsedThisTurn { get; set; }

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(3, ValueProp.Move),
        new IntVar("Hits", 2)
    ];

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner)
        {
            DipUsedThisTurn = false;
        }
        return Task.CompletedTask;
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
