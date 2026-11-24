using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Ancients;

public interface IOnAncientVisited
{
    Task OnAncientVisited(AncientEventModel ancient);
}
