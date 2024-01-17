using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class User
{
    public string id { get; set; }

    public string username { get; set; }

    public List<Unit> units { get; set; }

    public int next_unit_id;

    private Dictionary<Currency, int> currencies = new Dictionary<Currency, int>()
        {
            { Currency.Gold, 100 },
            { Currency.Gems, 100 },
            { Currency.Scrolls, 10 },
            { Currency.HeroicScrolls, 5 }
        };

    public int level = 1;

    public int experience = 0;

    public int experienceToNextLevel = 100;

    public UnityEvent OnCurrencyModified = new UnityEvent();
    public UnityEvent OnLevelModified = new UnityEvent();


    public string NextId() {
        string next_id = next_unit_id.ToString();
        next_unit_id = next_unit_id + 1;
        return next_id;
    }

    public void AddExperience(int experienceToAdd) {
        experience += experienceToAdd;

        // Level up
        while (experience >= experienceToNextLevel) {
            experience -= experienceToNextLevel;
            level++;
            experienceToNextLevel = (int)Math.Floor(Math.Pow((float)experienceToNextLevel, 1.1));
        }

        OnLevelModified.Invoke();
    }

    public int? GetCurrency(Currency name) {
        return currencies.ContainsKey(name) ? currencies[name] : null;
    }

    public void AddIndividualCurrency(Currency name, int amount) {
        if (currencies.ContainsKey(name)) {
            currencies[name] = currencies[name] + amount;
        } else {
            // User doesn't have this currency.
            if (amount < 0) { throw new InvalidOperationException("AddIndividualCurrency received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with BoxListItem.CanUserBuyItem."); }
            
            // Create it for him with the given amount.
            currencies.Add(name, amount);
        }
        OnCurrencyModified.Invoke();
    }

    public void AddCurrency(Dictionary<Currency, int> currencies) {
        User user = GlobalUserData.Instance.User;

        foreach (var currencyValue in currencies) {
            Currency currency = currencyValue.Key;
            int addAmount = currencyValue.Value;
            
            user.AddIndividualCurrency(currency, addAmount);
        }
    }

    public void SubstractCurrency(Dictionary<Currency, int> currencies) {
        Dictionary<Currency, int> negativeCurrencies = new Dictionary<Currency, int>();

        foreach (var pair in currencies)
        {
            negativeCurrencies.Add(pair.Key, -pair.Value);
        }

        AddCurrency(negativeCurrencies);
    }

    public void DeleteUnit(Unit unit) { 
        // We will need to unequip their items here
        units.Remove(unit);
    } 

    public bool FuseUnits(List<Unit> units) {
        Debug.Log("Fusing units: " + units.Count);
        if (CanFuseUnits(units)) {
            // Pop the first one. We will upgrade the quality of this guy.
            Unit mergeTarget = units[0];
            units.RemoveAt(0);

            // Upgrade!
            if (mergeTarget.QualityUp()) {
                // Delete all the other ones we fused into this one.
                foreach (Unit unit in units) { DeleteUnit(unit); }
                
                return true;
            }

            // Could not upgrade quality because target unit was invalid. Either it was Common or at its maximum possible quality value.
            return false;
        }

        // Units selected are invalid.
        return false;
    }

    // Check if the units list granted can be fused. Expects the head of the list to be the merge target.
    // The idea behind the requirement list implementation is that we can support different quality requirements, even for the same type (character or faction).
    private static bool CanFuseUnits(List<Unit> originalUnits) {
        // We don't want to alter the same units list
        List<Unit> units = new List<Unit>(originalUnits);

        // Pop the first one. We're looking to upgrade the quality of this guy.
        Unit mergeTarget = units[0];
        units.RemoveAt(0);
        
        // Get the required qualities of units with the same character
        List<Quality> sameCharacter = SameCharacterRequirements(mergeTarget.quality);
        if (sameCharacter.Count == 0) { return false; }

        // Get the required qualities of units with the same faction
        List<Quality> sameFaction = SameFactionRequirements(mergeTarget.quality);
        if (sameFaction.Count == 0) { return false; }

        foreach (Quality qualityReq in sameCharacter) {
            Unit evalUnit = units.Find(unit => unit.character.name == mergeTarget.character.name && unit.quality == qualityReq);
            if (evalUnit == null) {
                // Unmet quality requirement. Can't fuse units.
                return false;
            }
            // Remove it so we don't evaluate this one again.
            units.Remove(evalUnit);
        }

        foreach (Quality qualityReq in sameFaction) {
            Unit evalUnit = units.Find(unit => unit.character.faction == mergeTarget.character.faction && unit.quality == qualityReq);
            if (evalUnit == null) {
                // Unmet quality requirement. Can't fuse units.
                return false;
            }
            // Remove it so we don't evaluate this one again.
            units.Remove(evalUnit);
        }

        // If we got excess units then we also can't fuse.
        if (units.Count != 0) { return false; }

        return true;
    }

    public static List<Quality> SameCharacterRequirements(Quality targetQuality) {
        switch (targetQuality) {
            case Quality.Star4:
                return new List<Quality>{Quality.Star4, Quality.Star4};
            case Quality.Star5:
                return new List<Quality>{Quality.Star5};
            case Quality.Ilumination1:
                return new List<Quality>{Quality.Star5};
            case Quality.Ilumination2:
                return new List<Quality>{Quality.Star5};
            case Quality.Ilumination3:
                return new List<Quality>{Quality.Star5, Quality.Star5, Quality.Star5};
            default:
                return new List<Quality>();
        }
    }

    public static List<Quality> SameFactionRequirements(Quality targetQuality) {
        switch (targetQuality) {
            case Quality.Star4:
                return new List<Quality>{Quality.Star4, Quality.Star4, Quality.Star4, Quality.Star4};
            case Quality.Star5:
                return new List<Quality>{Quality.Star5, Quality.Star5, Quality.Star5, Quality.Star5};
            case Quality.Ilumination1:
                return new List<Quality>{Quality.Ilumination1};
            case Quality.Ilumination2:
                return new List<Quality>{Quality.Ilumination1, Quality.Ilumination1};
            case Quality.Ilumination3:
                return new List<Quality>{Quality.Ilumination1, Quality.Ilumination1};
            default:
                return new List<Quality>();
        }
    }
}
