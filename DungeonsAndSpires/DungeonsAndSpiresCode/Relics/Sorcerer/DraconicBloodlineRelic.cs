using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Map;
using MegaCrit.Sts2.Core.Models.Potions;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Relics.Sorcerer;

[Pool(typeof(SorcererRelicPool))]
public class DraconicBloodlineRelic : SubclassRelic
{
    public override RelicRarity Rarity => RelicRarity.None;
    
    //max hp
    public override async Task InitialBonus()
    {
        await CreatureCmd.GainMaxHp(Owner.Creature, 12m);
    }

    //gain fly after resting
    public override async Task Act2Bonus()
    {
        UnlockedFlight = true;
        CanFly = true;
    }

    private bool unlockedFlight = false;
    private bool canFly = false;
    public override bool ShouldAllowFreeTravel() => CanFly;

    [SavedProperty]
    public bool UnlockedFlight
    {
        get => unlockedFlight;
        set => unlockedFlight = value;
    }

    [SavedProperty]
    public bool CanFly
    {
        get => canFly;
        set => canFly = value;
    }

    public override async Task AfterRestSiteHeal(Player player, bool isMimicked)
    {
        if(!unlockedFlight)
            return;
        
        if(!isMimicked)
            CanFly = true;
        this.Status = RelicStatus.Active;
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (!this.CanFly || this.Owner.RunState.CurrentRoomCount > 1 || !(this.Owner.RunState is RunState runState) || runState.VisitedMapCoords.Count <= 1)
            return Task.CompletedTask;
        IReadOnlyList<MapCoord> visitedMapCoords = runState.VisitedMapCoords;
        MapCoord coord = visitedMapCoords[visitedMapCoords.Count - 2];
        MapPoint point = runState.Map.GetPoint(coord);
        if (point == null)
            return Task.CompletedTask;
        MapPoint currentMapPoint = this.Owner.RunState.CurrentMapPoint;
        if (currentMapPoint == null || point.Children.Contains(currentMapPoint))
            return Task.CompletedTask;
        this.CanFly = false;
        this.Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    //each turn, after casting 4 elemental spells gain 12 block
    public override async Task Act3Bonus()
    {
        unlockBlock = true;
    }

    public override bool ShowCounter => unlockBlock;
    public override int DisplayAmount => ElementsPlayed;

    private bool unlockBlock = false;
    private int _elementsPlayed;

    [SavedProperty]
    public bool UnlockedBlock
    {
        get { return unlockBlock; }
        set => unlockBlock = value;
    }
    
    [SavedProperty]
    public int ElementsPlayed 
    {
        get { return this._elementsPlayed; }
        set
        {
            this._elementsPlayed = value;
            this.InvokeDisplayAmountChanged();
        }
    }

    private bool _blockedThisTurn = false;

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner != Owner)
            return;
        if(_blockedThisTurn)
            return;
        
        if (cardPlay.Card.HasElementKeyword())
        {
            ElementsPlayed++;
        }

        if (_elementsPlayed >= 4)
        {
            CreatureCmd.GainBlock(Owner.Creature, 12m, ValueProp.Unpowered, null);
            ElementsPlayed = 0;
            _blockedThisTurn  = true;
        }
        this.InvokeDisplayAmountChanged();
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == CombatSide.Player)
            _blockedThisTurn = false;
    }
}