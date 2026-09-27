using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class BladeOfDisaster() : SpellCard(8, 2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(12, ValueProp.Unpowered),
        new IntVar("Hits", 2)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        int hits = DynamicVars["Hits"].IntValue;
        decimal damage = DynamicVars["Damage"].GetCalculatedValue();
        for (int i = 0; i < hits; i++)
        {
            await CreatureCmd.Damage(choiceContext, cardPlay.Target, damage, ValueProp.Unpowered, Owner.Creature, this, cardPlay);
        }
    }

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }
        if (Owner.PlayerCombatState?.HasAvailableSlotForLevel(8) != true)
        {
            return;
        }
        if (PileType.Discard.GetPile(Owner).Cards.Contains(this))
        {
            await CardCmd.AutoPlay(choiceContext, this, null);
        }
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card == this && cardPlay.IsAutoPlay)
        {
            return;
        }
        await base.AfterCardPlayed(choiceContext, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Damage.UpgradeValueBy(4m);
    }
}