using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.SimpleWeapons;


public class QuarterstaffSpellfocus() : SimpleWeaponCard(2, CardType.Skill, CardRarity.Rare, TargetType.Self)
{
    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new IntVar("AbilityPotency", 1)
    ];

protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PotencyCmd.Add(choiceContext, Owner, DynamicVars["AbilityPotency"].IntValue);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
    {
        if (card == this)
        {
            await GainRandomSpell(choiceContext);
        }
    }
    public override async Task BeforeCardRemoved(CardModel card)
    {
        if (card == this)
        {
            await GainRandomSpell(null);
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Scrolled)
    ];

    private async Task GainRandomSpell(PlayerChoiceContext? choiceContext)
    {
        if (CombatState == null)
        {
            return;
        }

        var spells = ModelDb.CardPool<DASSpellCardPool>().AllCards.Where(c => c is SpellCard).ToList();
        if (spells.Count == 0)
        {
            return;
        }

        var card = spells[Owner.RunState.Rng.CombatCardSelection.NextInt(spells.Count)];
        var created = CombatState.CreateCard(card, Owner);
        created.AddKeyword(CoreKeywords.Scrolled);
        await CardPileCmd.AddGeneratedCardToCombat(created, PileType.Hand, Owner);
    }

    protected override void OnUpgrade()
    {
        this.DynamicVars["AbilityPotency"].UpgradeValueBy(1M);
    }
}