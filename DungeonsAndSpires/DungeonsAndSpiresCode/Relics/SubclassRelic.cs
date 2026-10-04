using MegaCrit.Sts2.Core.Saves.Runs;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics;

public abstract class SubclassRelic : DungeonsAndSpiresRelic
{
    public override async Task AfterActEntered()
    {
        switch (CurrentLevel)
        {
            case 0: await InitialBonus();
                CurrentLevel++; 
                break;
            case 1: await Act2Bonus(); 
                CurrentLevel++;
                break;
            case 2: await Act3Bonus();
                CurrentLevel++; 
                break;
        }
    }

    private int _currentLevel = 0;

    [SavedProperty]
    public int CurrentLevel
    {
        get => _currentLevel;
        set => _currentLevel = value;
    }
    
    public abstract Task InitialBonus();
    public abstract Task Act2Bonus();
    public abstract Task Act3Bonus();
}