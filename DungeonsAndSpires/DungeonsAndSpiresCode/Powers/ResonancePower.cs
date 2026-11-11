using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.ExtraSimpleWeapons;
using DungeonsAndSpires.DungeonsAndSpiresCode.CombatHistoryEntries;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Powers;

// When i reach 4 stacks, the next Attack/Skill/Power gives you 1 Strength/Dex/Both, plus 1 energy
public class ResonancePower : DungeonsAndSpiresPower
{
    public const int Threshold = 4;

    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override int DisplayAmount => Amount;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar("energy", 1)];

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Player != Owner.Player || Amount < Threshold)
        {
            DynamicVars["energy"].BaseValue = 1 + (Owner.GetPower<MaceLeechingPower>()?.Amount ?? 0);
            return;
        }
        switch (cardPlay.Card.Type)
        {
            case CardType.Attack:
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 1, Owner, null);
                break;
            case CardType.Skill:
                await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 1, Owner, null);
                break;
            case CardType.Power:
                await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, 1, Owner, null);
                await PowerCmd.Apply<DexterityPower>(choiceContext, Owner, 1, Owner, null);
                break;
            default:
                return;
        }
        
        // and uh Leeching gives extra energy
        int energy = 1 + (Owner.GetPower<MaceLeechingPower>()?.Amount ?? 0);
        await PlayerCmd.GainEnergy(energy, Owner.Player);
        await PowerCmd.ModifyAmount(choiceContext, this, -Threshold, null, null);
        if (Owner.CombatState != null)
        {
            CombatManager.Instance.History.Record(Owner.CombatState,
                new ResonanceActivationEntry(Owner, Owner.CombatState.RoundNumber, Owner.CombatState.CurrentSide,
                    CombatManager.Instance.History, Owner.CombatState.Players));
        }

        // farcry makes the party's cards cheaper
        Owner.GetPower<MaceFarcryPower>()?.GrantDiscount();

        // the activation applies reverb
        var radiance = Owner.GetPower<MaceRadiancePower>();
        if (radiance != null && Owner.CombatState != null)
        {
            await PowerCmd.Apply<Reverberation>(choiceContext, Owner.CombatState.HittableEnemies, radiance.Amount, Owner, null);
        }

        // draws card upon activation
        var concentrate = Owner.GetPower<MaceConcentratePower>();
        if (concentrate != null && Owner.Player != null)
        {
            await CardPileCmd.Draw(choiceContext, concentrate.Amount, Owner.Player);
            await PowerCmd.Remove(concentrate);
        }

        // parralells go from discard to top draw
        if (Owner.Player != null)
        {
            foreach (var parallel in PileType.Discard.GetPile(Owner.Player).Cards.OfType<MaceParallel>().ToList())
            {
                await CardPileCmd.Add(parallel, PileType.Draw, CardPilePosition.Top);
            }
        }
    }
}
