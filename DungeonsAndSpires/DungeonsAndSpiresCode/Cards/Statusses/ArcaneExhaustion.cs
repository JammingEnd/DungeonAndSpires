using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Statusses;
[Pool(typeof(StatusCardPool))]
public class ArcaneExhaustion() : CoreCard(0, CardType.Status, CardRarity.Status, TargetType.Self)
{
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Unplayable
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("Level", 0)
    ];

    public int Level { get; private set; }

    public void SetLevel(int level)
    {
        Level = level;
        DynamicVars["Level"].BaseValue = level;
    }
    
    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card == this)
        {
            await PotencyCmd.Remove(choiceContext, this.Owner, Level);
        }
    }

    public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
    {
        if (card == this)
        {
            this.Owner.PlayerCombatState.GetPotencyState().Current += Level;
        }
    }
}