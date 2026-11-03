using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
public class MagicWindsPower : DungeonsAndSpiresPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Single;

    private bool _isFree = false;

    public void MakeFree(bool upgraded)
    {
        _isFree = upgraded;
    }
    
    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player)
            return;
        if(cardPlay.Card is MpMagicWinds)
            return;
        if (cardPlay.Card is SpellCard spellCard)
        {
            if (spellCard.Level == 0)
            {
                foreach (var ally in CombatState.PlayerCreatures)
                {
                    if(ally.IsDead)
                        continue;
                    if(ally == Owner)
                        return;
                    var card = cardPlay.Card.CreateCloneForPlayer(ally.Player);
                    if(_isFree)
                        card.EnergyCost.SetThisCombat(0);
                    
                    await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner.Player);
                }

                await PowerCmd.Remove(this);
            }
        }
    }
}