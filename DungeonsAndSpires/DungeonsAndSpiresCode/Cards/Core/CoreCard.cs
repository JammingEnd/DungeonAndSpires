using BaseLib.Abstracts;
using BaseLib.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.AbilityPotency;
using DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Spells;
using DungeonsAndSpires.DungeonsAndSpiresCode.Extensions;
using DungeonsAndSpires.DungeonsAndSpiresCode.Keywords;
using DungeonsAndSpires.DungeonsAndSpiresCode.Tags;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System.Text.RegularExpressions;
using MegaCrit.Sts2.Core.ValueProps;

namespace DungeonsAndSpires.DungeonsAndSpiresCode.Cards.Core;

public abstract class CoreCard(int cost, CardType type, CardRarity rarity, TargetType target) :
    CustomCardModel(cost, type, rarity, target), ICustomTypeTextCard
{
    //Image size:
    //Normal art: 1000x760 (Using 500x380 should also work, it will simply be scaled.)
    //Full art: 606x852
    private static readonly Regex SnakeRegex = new("([a-z0-9])([A-Z])");

    private string CardName() => SnakeRegex.Replace(GetType().Name, "$1_$2").ToLowerInvariant();

    public override string CustomPortraitPath => $"{CardName()}.png".CardImagePath();

    //Smaller variants of card images for efficiency:
    //Smaller variant of fullart: 250x350
    //Smaller variant of normalart: 250x190

    //Uses card_portraits/card_name.png as image path. These should be smaller images.
    public override string PortraitPath => $"{CardName()}.png".CardImagePath();
    public override string BetaPortraitPath => $"beta/{CardName()}.png".CardImagePath();
    
    //so this is in corecard because i want to have stuff like "dip your sword in fire" or stuff like that. 
    public virtual IEnumerable<CardKeyword> ElementOptions => [];

    // Plaque shows "{cardtype} | {type}".
    public IEnumerable<LocString> GetTypeModifiers()
    {
        if (Tags.Contains(DASCoreCardtags.Weapon))
        {
            return [new LocString("cards", "DUNGEONSANDSPIRES-PLAQUE_WEAPON")];
        }
        if (Tags.Contains(DASCoreCardtags.Spell))
        {
            return [new LocString("cards", "DUNGEONSANDSPIRES-PLAQUE_SPELL")];
        }
        if (Tags.Contains(DASCoreCardtags.Unarmed))
        {
            return [new LocString("cards", "DUNGEONSANDSPIRES-PLAQUE_UNARMED")];
        }
        return [];
    }
    
    //does the card contains a valid Imbued element. logic is used for Chromatic orb and future weapon attacks that allows 
    // dipping weapon in poison or fire or somethingg
    public CardKeyword? ImbuedElement => ElementOptions.FirstOrDefault(Keywords.Contains);
    
    public void Imbue(CardKeyword keyword)
    {
        foreach (var element in ElementOptions)
        {
            if (Keywords.Contains(element))
            {
                RemoveKeyword(element);
            }
        }
        AddKeyword(keyword);
    }

    // For Potentcy. only when potentcy is 1+ should it be visible and not dissapear
    protected override void AddExtraArgsToDescription(LocString description)
    {
        description.Add("HasPotencyStep", PotencyVars.Count > 0 || DynamicVars.ContainsKey("PotencyStep"));
    }

    /// <summary>
    /// Maps a DynamicVar name to its potency scaling: (step, mult). Every <c>step</c> Ability
    /// Potency, the var increases by <c>mult</c>. A step of 1 is "Potent 1".
    /// </summary>
    public virtual Dictionary<string, (int step, int mult)> PotencyVars => [];

    /// <summary>
    /// The extra potency level granted to a <c>step</c> var by the Heightened keyword, matching
    /// the bonus applied in <see cref="BuildCanonicalVars"/>.
    /// </summary>
    protected int HeightenedLevelBonus(int step) =>
        Keywords.Contains(CoreKeywords.Heightened) ? (step == 1 ? 3 : 1) : 0;

    /// <summary>
    /// The card's base (unscaled) vars. Potency scaling is applied automatically to any var whose
    /// name appears in <see cref="PotencyVars"/>.
    /// </summary>
    protected virtual IEnumerable<DynamicVar> CardVars => [];

    protected override IEnumerable<DynamicVar> CanonicalVars => BuildCanonicalVars();
    
    private IEnumerable<DynamicVar> BuildCanonicalVars()
    {
        var result = new List<DynamicVar>();
        if (PotencyVars.Count > 0)
        {
            result.Add(new IntVar("PotencyStep", PotencyVars.Values.First().step));
        }

        foreach (var item in CardVars)
        {
            if (PotencyVars.TryGetValue(item.Name, out var potency) && potency.step > 0)
            {
                decimal baseValue = item.BaseValue;
                result.AddRange(MakeCalculatedVar(
                    item.Name,
                    (int)baseValue,
                    (model, creature) =>
                    {
                        decimal playerPotency = model.Owner.PlayerCombatState?.GetPotency() ?? 0;
                        decimal level = Math.Floor(playerPotency / potency.step);
                        // Heightened cards have their potency level met by 1 (or gain 3 if their potency step is 1).
                        if (model.Keywords.Contains(CoreKeywords.Heightened))
                        {
                            level += potency.step == 1 ? 3 : 1;
                        }
                        return level;
                    },
                    potency.mult));
            }
            else
            {
                result.Add(item);
            }
        }
        
        return result;
    }

    public virtual decimal GetElementEffectAmount(CardKeyword element)
    {
        var varName = GetElementEffectVarName(element);
        
        // gotta love TryGetValue inshallah
        return varName != null && DynamicVars.TryGetValue(varName, out var v) ? v.GetCalculatedValue() : 0;
    }
    
    // this shows when playing chromatic orb or when dipping a weapon
    public async Task<CardKeyword?> ChooseElement(PlayerChoiceContext choiceContext)
    {
        if (!ElementOptions.Any())
        {
            return null;
        }

        var options = new List<CardModel>();
        foreach (var element in ElementOptions)
        {
            options.Add(CreateElementToken(element));
        }

        var prefs = new CardSelectorPrefs(new LocString("cards", "DUNGEONSANDSPIRES-ELEMENTAL_SELECTION.selectionPrompt"), 1, 1);
        var picked = await CardSelectCmd.FromSimpleGrid(choiceContext, options, Owner, prefs);
        var token = picked.FirstOrDefault();
        if (token == null)
        {
            return null;
        }

        var keyword = ElementOptions.FirstOrDefault(element => token.Keywords.Contains(element));
        if (keyword != null)
        {
            Imbue(keyword);
        }
        return keyword;
    }
    
    // so acid and poison arent a damage type, they apply something (poison and vulnerable), this is the method for that
    public async Task ApplyImbuedElementEffect(PlayerChoiceContext choiceContext, Creature target)
    {
        if (ImbuedElement is not CardKeyword element)
        {
            return;
        }

        decimal amount = GetElementEffectAmount(element);
        if (amount <= 0)
        {
            return;
        }

        if (element == CoreKeywords.Poison)
        {
            await PowerCmd.Apply<PoisonPower>(choiceContext, target, amount, Owner.Creature, this);
        }
        else if (element == CoreKeywords.Acid)
        {
            await PowerCmd.Apply<VulnerablePower>(choiceContext, target, amount, Owner.Creature, this);
        }
    }

    private static string? GetElementEffectVarName(CardKeyword element)
    {
        if (element == CoreKeywords.Poison)
        {
            return "PoisonPower";
        }
        if (element == CoreKeywords.Acid)
        {
            return "VulnerablePower";
        }
        return null;
    }
    
    //teehee big token energy
    private CardModel CreateElementToken(CardKeyword element)
    {
        CardModel canonical;
        if (element == CoreKeywords.Fire)
        {
            canonical = ModelDb.Card<ElementFire>();
        }
        else if (element == CoreKeywords.Cold)
        {
            canonical = ModelDb.Card<ElementIce>();
        }
        else if (element == CoreKeywords.Thunder)
        {
            canonical = ModelDb.Card<ElementThunder>();
        }
        else if (element == CoreKeywords.Lightning)
        {
            canonical = ModelDb.Card<ElementLightning>();
        }
        else if (element == CoreKeywords.Acid)
        {
            canonical = ModelDb.Card<ElementAcid>();
        }
        else if (element == CoreKeywords.Poison)
        {
            canonical = ModelDb.Card<ElementPoison>();
        }
        else
        {
            throw new ArgumentOutOfRangeException(nameof(element), element, "No element token card for element");
        }

        // i am required to do this because without it i have an immutable (which is a big nono)
        var token = Owner.RunState.CreateCard(canonical, Owner);

        var varName = GetElementEffectVarName(element);
        if (varName != null && token.DynamicVars.TryGetValue(varName, out var dv))
        {
            decimal amount = GetElementEffectAmount(element);
            dv.BaseValue = amount;
            dv.EnchantedValue = amount;
            dv.PreviewValue = amount;
        }

        return token;
    }

    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource,
        CardPlay? cardPlay)
    {
        if (cardSource != this || dealer != Owner.Creature)
        {
            return 0;
        }
        // finesse weapons use dex for damage instead of str+vigor, whichever is higher.
        if (Keywords.Contains(CoreKeywords.Finesse) && props.IsPoweredAttack())
        {
            int strength = Owner.Creature.GetPowerAmount<StrengthPower>();
            int vigor = Owner.Creature.GetPowerAmount<VigorPower>();
            int dexterity = Owner.Creature.GetPowerAmount<DexterityPower>();
            return Math.Max(0, dexterity - (strength + vigor));
        }
        return 0;
    }

    // Potent attack spells are unpowered, so Weak normally skips them; apply the reduction here.
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (cardSource != this || dealer != Owner.Creature || target == null)
        {
            return 1m;
        }
        // Weak already handles powered attacks; avoid double-applying.
        if (props.IsPoweredAttack())
        {
            return 1m;
        }
        if (Type == CardType.Attack && Owner.Creature.GetPowerAmount<WeakPower>() > 0)
        {
            return 0.75m;
        }
        return 1m;
    }
}