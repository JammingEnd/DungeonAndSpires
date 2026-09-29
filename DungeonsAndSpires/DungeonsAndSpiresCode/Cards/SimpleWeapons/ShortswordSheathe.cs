using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class ShortswordSheathe() : SimpleWeaponCard(1, CardType.Skill, CardRarity.Common, TargetType.Self)
{
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-SHORTSWORD_SHEATHE.selectionPrompt"), 1, 1);
        Func<CardModel, bool> filter = c => c.Tags.Contains(DASCoreCardtags.Weapon);
        var selected = await CardSelectCmd.FromHand(choiceContext, Owner, prefs, filter, this);
        var target = selected.FirstOrDefault();
        if (target == null)
        {
            return;
        }

        await CardCmd.Discard(choiceContext, target);
        int energy = target.EnergyCost.Canonical + (IsUpgraded ? 1 : 0);
        await PlayerCmd.GainEnergy(energy, Owner);
    }
}