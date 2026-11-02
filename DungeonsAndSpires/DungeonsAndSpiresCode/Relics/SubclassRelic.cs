using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Relics;
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
    
    //instead of a patch, im just gonna to class-specific combat starts here
    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains<Creature>(this.Owner.Creature) || Owner.PlayerCombatState.TurnNumber > 1)
            return;

        if (Owner.Character is SorcererCharacter sorcerer)
        {
            
        }
        if (Owner.Character is FighterCharacter fighter)
        {
            await PowerCmd.Apply<FighterSecondWindPower>(choiceContext, Owner.Creature, FighterSecondWindPower.AttacksRequired, Owner.Creature, null);
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