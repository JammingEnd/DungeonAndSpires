namespace DungeonsAndSpires.DungeonsAndSpiresCode.SpellSlots;

public class SpellslotsState
{
    public SpellslotsState(bool isHalfcaster = false)
    {
        if (isHalfcaster)
        {
            // half caster spellslots 
            Spellslots = new()
            {
                { 1, new Spellslot(2) },
                { 2, new Spellslot(2) },
                { 3, new Spellslot(2) },
                { 4, new Spellslot(1) },
                { 5, new Spellslot(1) },
            };
        }
        else
        {
            // full caster slots
            Spellslots = new()
            {
                { 1, new Spellslot(3) },
                { 2, new Spellslot(3) },
                { 3, new Spellslot(3) },
                { 4, new Spellslot(2) },
                { 5, new Spellslot(2) },
                { 6, new Spellslot(2) },
                { 7, new Spellslot(1) },
                { 8, new Spellslot(1) },
                { 9, new Spellslot(1) },
            };
        }
    }

    public readonly Dictionary<int, Spellslot> Spellslots;
}

public class Spellslot(int initialMax)
{
    private int max = initialMax;
    private int current = initialMax;

    public void SetMax(int max) => this.max = max;
    public void SetCurrent(int current) => this.current = current;
    public int GetMax() => max;
    public int GetCurrent() => current;

    public void Consume(int amount = 1)
    {
        current = Math.Max(0, current - amount);
    }

    public void Add(int amount = 1)
    {
        current = Math.Min(max, current + amount);
    }
}