using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.RestSite;

public class SpellSelectionRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public override async Task<bool> OnSelect()
    {
        var runState = RunManager.Instance.State;
        if (runState == null)
        {
            return false;
        }

        foreach (var model in runState.IterateHookListeners(null).OfType<IOnRestSiteSpellOption>())
        {
            await model.OnRestSiteSpellSelected(Owner);
            ((AbstractModel)model).InvokeExecutionFinished();
        }

        var healamount = (decimal)(Owner.Creature.MaxHp * 0.15);
        await CreatureCmd.Heal(Owner.Creature, healamount);
        return true;
    }
    
    public override string OptionId => "SELECT_SPELLS";
}