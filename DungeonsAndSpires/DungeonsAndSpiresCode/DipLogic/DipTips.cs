using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.DipLogic;

public static class DipTips
{
    public static IHoverTip Dip => new HoverTip(
        new LocString("static_hover_tips", "DUNGEONSANDSPIRES-DIP.title"),
        new LocString("static_hover_tips", "DUNGEONSANDSPIRES-DIP.description"));
}
