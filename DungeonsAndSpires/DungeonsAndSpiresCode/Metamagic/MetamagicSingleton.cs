using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Metamagic;

public class MetamagicSingleton() : CustomSingletonModel(HookType.Combat)
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
       await MetamagicCmd.Set(choiceContext, player, 4);
    }
}