using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class LeamundsSecretChest() : SpellCard(4, 0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new CardsVar("Exhaust", 2),
        new IntVar("MaxLevel", 5)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-LEAMUNDS_SECRET_CHEST.selectionPrompt"), 0, DynamicVars["Exhaust"].IntValue);
        Func<CardModel, bool> filter = c => c is not SpellCard;
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, filter, this);
        foreach (var card in selected)
        {
            await CardCmd.Exhaust(choiceContext, card);
        }

        int maxLevel = DynamicVars["MaxLevel"].IntValue;
        for (int level = maxLevel; level >= 1; level--)
        {
            var slot = Owner.PlayerCombatState?.GetSpellslotForLevel(level);
            if (slot != null && slot.GetCurrent() < slot.GetMax())
            {
                await SpellslotsCmd.AddSpellSlotForLevel(choiceContext, Owner, level);
                break;
            }
        }

        await PowerCmd.Apply<EnergyNextTurnPower>(choiceContext, Owner.Creature, 2, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars["MaxLevel"].UpgradeValueBy(1);
    }
}