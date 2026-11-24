using MegaCrit.Sts2.Core.Entities.Players;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.RestSite;

public interface IOnRestSiteSpellOption
{
    Task OnRestSiteSpellSelected(Player player);
}