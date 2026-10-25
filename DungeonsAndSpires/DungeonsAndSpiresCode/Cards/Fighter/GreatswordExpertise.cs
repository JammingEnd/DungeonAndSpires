using BaseLib.Abstracts;
using BaseLib.Utils;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;
using DungeonsAndSpires.DungeonsAndSpiresCode.Character;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Fighter;

[Pool(typeof(FighterCardPool))]
public class GreatswordExpertise() : MartialWeaponCard(1, CardType.Skill, CardRarity.Basic, TargetType.Self), ITranscendenceCard
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CoreKeywords.TwoHanded];

    public override TwoHandedPower? TwoHandedPower => ModelDb.Power<GreatswordTwoHandedPower>();

    protected override IEnumerable<DynamicVar> CardVars =>
    [
        new BlockVar(7, ValueProp.Move),
        new IntVar("Switch", 1)
    ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await PowerCmd.Apply<SwitchPower>(choiceContext, Owner.Creature, DynamicVars["Switch"].IntValue, Owner.Creature, this);
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [
        HoverTipFactory.FromPower<SwitchPower>()
    ];

    protected override void OnUpgrade()
    {
        DynamicVars["Switch"].UpgradeValueBy(1);
    }
    public CardModel GetTranscendenceTransformedCard()
    {
        return ModelDb.Get<GreatswordBattleflow>();
    }
}
