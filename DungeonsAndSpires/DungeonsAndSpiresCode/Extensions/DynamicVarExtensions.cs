using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;

public static class DynamicVarExtensions
{
    /// <summary>
    /// Returns the value of this var. For <see cref="CalculatedVar"/>s (e.g. from
    /// MakeCalculatedVar) this returns the fully calculated value (base + extra * multiplier);
    /// for plain vars it returns BaseValue. Pass null for cards that hit multiple/no targets.
    /// </summary>
    public static decimal GetCalculatedValue(this DynamicVar var, Creature? target = null)
    {
        return var is CalculatedVar calculated ? calculated.Calculate(target) : var.BaseValue;
    }
}