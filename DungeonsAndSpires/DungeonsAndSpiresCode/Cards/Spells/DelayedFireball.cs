using BaseLib.Abstracts;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;


public class DelayedFireball() : SpellCard(8, 3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
{
    public override Dictionary<string, (int step, int mult)> PotencyVars => new()
    {
        { "Damage", (5, 1) },
        { "Increase", (5, 1) }
    };

    public override IEnumerable<CardKeyword> CanonicalKeywords =>
    [
        CardKeyword.Retain
    ];

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new DamageVar(7, ValueProp.Unpowered),
        new IntVar("Increase", 7),
        ..MakeCalculatedVar("CurrentDamage", 0, (card, target) =>
        {
            decimal damage = card.DynamicVars["Damage"].GetCalculatedValue();
            decimal increase = card.DynamicVars["Increase"].GetCalculatedValue();
            decimal turns = card is DelayedFireball fireball ? fireball._turnsInHand : 0;
            return damage + increase * turns;
        }, 1)
    ];

    private int _turnsInHand;

    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
        {
            return;
        }
        if (PileType.Hand.GetPile(Owner).Cards.Contains(this))
        {
            _turnsInHand++;
        }
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        decimal total = DynamicVars["CurrentDamage"].GetCalculatedValue(cardPlay.Target);
        await CreatureCmd.Damage(choiceContext, cardPlay.Target, total, ValueProp.Unpowered, Owner.Creature, this, cardPlay);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromKeyword(CoreKeywords.Potent)
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["DamageBase"].UpgradeValueBy(1m);
        DynamicVars["IncreaseBase"].UpgradeValueBy(1m);
    }
}