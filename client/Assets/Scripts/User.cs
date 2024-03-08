using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Specialized;

[Serializable]
public class User
{
    public string id { get; set; }

    public string username { get; set; }

    public List<Unit> units { get; set; }

    public List<Item> items { get; set; }

    public int next_unit_id;

    public Dictionary<Currency, int> currencies;

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

	public List<(string campaignId, string levelId)> campaignsProgress = new List<(string, string)>();

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

    // This method should be unified with AddIndividualCurrency, only one of these should exist
    public void SetCurrencyAmount(Currency currency, int amount) {
        if (currencies.ContainsKey(currency)) {
            currencies[currency] = amount;
        } else {
            // User doesn't have this currency.
            if (amount < 0) { throw new InvalidOperationException("SetCurrencyAmount received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with User.CanAfford()"); }
            
            // Create it for him with the given amount.
            currencies.Add(currency, amount);
        }
        OnCurrencyModified.Invoke();
    }

    public void SubtractCurrency(Dictionary<Currency, int> currencies) {
        Dictionary<Currency, int> negativeCurrencies = new Dictionary<Currency, int>();

        foreach (var pair in currencies)
        {
            negativeCurrencies.Add(pair.Key, -pair.Value);
        }

        AddCurrency(negativeCurrencies);
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
