using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Ancients;

public interface IOnAncientDialogueDone
{
    Task OnAncientDialogueDone(AncientEventModel ancient);
}
