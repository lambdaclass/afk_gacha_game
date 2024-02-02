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

    private DateTime lastAfkRewardClaim = DateTime.Now;
    private DateTime lastAfkRewardAccum = DateTime.Now;

    private Dictionary<Currency, int> accumulatedCurrencyReward = new Dictionary<Currency, int>();

    public int accumulatedExperienceReward = 0;

    public Dictionary<Currency, int> afkMaxCurrencyReward = new Dictionary<Currency, int>();
    public int afkMaxExperienceReward = 0;

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

    public int GetCurrencyAfkReward(Currency name) {
        return accumulatedCurrencyReward.ContainsKey(name) ? accumulatedCurrencyReward[name] : 0;
    }

    public int GetMaxCurrencyReward(Currency name) {
        return afkMaxCurrencyReward.ContainsKey(name) ? afkMaxCurrencyReward[name] : 0;
    }

    public void AddIndividualCurrency(Currency name, int amount) {
        if (currencies.ContainsKey(name)) {
            currencies[name] = currencies[name] + amount;
        } else {
            // User doesn't have this currency.
            if (amount < 0) { throw new InvalidOperationException("AddIndividualCurrency received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with User.CanAfford()"); }
            
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
        if (CanFuseUnits(units)) {
            // Pop the first one. We will upgrade the rank of this guy.
            Unit mergeTarget = units[0];
            units.RemoveAt(0);

            // Upgrade!
            if (mergeTarget.RankUp()) {
                // Delete all the other ones we fused into this one.
                foreach (Unit unit in units) { DeleteUnit(unit); }
                
                return true;
            }

            // Could not upgrade rank because target unit was invalid. Either it was Common or at its maximum possible rank value.
            return false;
        }

        // Units selected are invalid.
        return false;
    }

    // Check if the units list granted can be fused. Expects the head of the list to be the merge target.
    // The idea behind the requirement list implementation is that we can support different rank requirements, even for the same type (character or faction).
    private static bool CanFuseUnits(List<Unit> originalUnits) {
        // We don't want to alter the same units list
        List<Unit> units = new List<Unit>(originalUnits);

        // Pop the first one. We're looking to upgrade the rank of this guy.
        Unit mergeTarget = units[0];
        units.RemoveAt(0);
        
        // Get the required qualities of units with the same character
        List<Rank> sameCharacter = SameCharacterRequirements(mergeTarget.rank);
        if (sameCharacter.Count == 0) { return false; }

        // Get the required qualities of units with the same faction
        List<Rank> sameFaction = SameFactionRequirements(mergeTarget.rank);
        if (sameFaction.Count == 0) { return false; }

        foreach (Rank rankReq in sameCharacter) {
            Unit evalUnit = units.Find(unit => unit.character.name == mergeTarget.character.name && unit.rank == rankReq);
            if (evalUnit == null) {
                // Unmet rank requirement. Can't fuse units.
                return false;
            }
            // Remove it so we don't evaluate this one again.
            units.Remove(evalUnit);
        }

        foreach (Rank rankReq in sameFaction) {
            Unit evalUnit = units.Find(unit => unit.character.faction == mergeTarget.character.faction && unit.rank == rankReq);
            if (evalUnit == null) {
                // Unmet rank requirement. Can't fuse units.
                return false;
            }
            // Remove it so we don't evaluate this one again.
            units.Remove(evalUnit);
        }

        // If we got excess units then we also can't fuse.
        if (units.Count != 0) { return false; }

        return true;
    }

    public static List<Rank> SameCharacterRequirements(Rank targetRank) {
        switch (targetRank) {
            case Rank.Star4:
                return new List<Rank>{Rank.Star4, Rank.Star4};
            case Rank.Star5:
                return new List<Rank>{Rank.Star5};
            case Rank.Illumination1:
                return new List<Rank>{Rank.Star5};
            case Rank.Illumination2:
                return new List<Rank>{Rank.Star5};
            case Rank.Illumination3:
                return new List<Rank>{Rank.Star5, Rank.Star5, Rank.Star5};
            default:
                return new List<Rank>();
        }
    }

    public static List<Rank> SameFactionRequirements(Rank targetRank) {
        switch (targetRank) {
            case Rank.Star4:
                return new List<Rank>{Rank.Star4, Rank.Star4, Rank.Star4, Rank.Star4};
            case Rank.Star5:
                return new List<Rank>{Rank.Star5, Rank.Star5, Rank.Star5, Rank.Star5};
            case Rank.Illumination1:
                return new List<Rank>{Rank.Illumination1};
            case Rank.Illumination2:
                return new List<Rank>{Rank.Illumination1, Rank.Illumination1};
            case Rank.Illumination3:
                return new List<Rank>{Rank.Illumination1, Rank.Illumination1};
            default:
                return new List<Rank>();
        }
    }

    public void AccumulateAFKRewards(){
        DateTime now = DateTime.Now;

        // To cap at 12 hours
        DateTime lastClaim = lastAfkRewardClaim;
        TimeSpan maxTimeSpan = new TimeSpan(12, 0, 0);
        // To calculate this interval's rewards
        DateTime lastAccum = lastAfkRewardAccum;

        // This way we cap the rewards to our maximums
        TimeSpan maxTimeSpanAccumulable = now - lastClaim;
        // Get the minimum between the two
        if (maxTimeSpanAccumulable.CompareTo(maxTimeSpan) == 1) { maxTimeSpanAccumulable = maxTimeSpan; }

        // How much time happened since we last accumulated
        TimeSpan timeSinceLastAccum = now - lastAccum;

        // Get the minimum between the two
        if (timeSinceLastAccum.CompareTo(maxTimeSpanAccumulable) == 1) { timeSinceLastAccum = maxTimeSpanAccumulable; }

        // Number between 0 and 1, we multiply this to the maximum rewards possible to calculate the corresponding amount
        double multiplier = Math.Min(timeSinceLastAccum.Divide(new TimeSpan(12, 0, 0)), (1));

        // Calculate rewards
        int xpToAccum = (int)Math.Round(afkMaxExperienceReward * multiplier);
        Dictionary<Currency, int> currencyToAccum = new Dictionary<Currency, int>();
        foreach (var currReward in afkMaxCurrencyReward) {
            currencyToAccum.Add(currReward.Key, (int)Math.Round(currReward.Value * multiplier));
        }

        // Update rewards
        accumulatedExperienceReward += xpToAccum;
        foreach (var currReward in currencyToAccum) {
            Currency name = currReward.Key;
            int value = currReward.Value;

            if (accumulatedCurrencyReward.ContainsKey(name)) {
                accumulatedCurrencyReward[name] = accumulatedCurrencyReward[name] + value;
            } else {
                accumulatedCurrencyReward.Add(name, value);
            }
        }

        lastAfkRewardAccum = now;
    }

    public void ClaimAFKRewards() {
        AccumulateAFKRewards();
        AddCurrency(accumulatedCurrencyReward);
        AddExperience(accumulatedExperienceReward);
        accumulatedCurrencyReward = new Dictionary<Currency, int>();
        accumulatedExperienceReward = 0;
    }

    public bool CanAfford(Dictionary<Currency, int> itemCosts) {
        // Check if the player has enough of each currency
        foreach (var cost in itemCosts) {
            Currency currency = cost.Key;
            int costAmount = cost.Value;

            int? money = GetCurrency(currency);

            if (money == null) { return false; }
            if (money < costAmount) { return false; }
        }

        // Player has enough of all currencies
        return true;
    }
}
