using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Singletons;

public class WishSingleton() : CustomSingletonModel(HookType.Combat)
{
    public readonly Dictionary<Player, bool> PlayersWishStatus = new();
    
    public override async Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        
        if (power is WishPower)
        {
            PlayersWishStatus[applier.Player] = true;
        }

        foreach (var player in PlayersWishStatus.Keys)
        {
            
        }
    }
}