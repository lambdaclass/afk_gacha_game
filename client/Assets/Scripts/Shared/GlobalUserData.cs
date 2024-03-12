using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalUserData : MonoBehaviour
{
    // This should be in it's own manager like of Curse of Mirra, not a list in the user singleton (should there be a user singleton?)
    [SerializeField]
    List<Character> characters;

    public List<Character> AvailableCharacters {
        get {
            return this.characters;
        }
    }

    [SerializeField]
    List<ItemTemplate> itemtemplates;
    public List<ItemTemplate> AvailableItemTemplates {
        get {
            return this.itemtemplates;
        }
    }

    // Singleton instance
    private static GlobalUserData instance;

    // User
    private User user;

    // Public property to access the user
    public User User
    {
        get { return user; }
        set { user = value; }
    }

    // Public property to access the user's units
    public List<Unit> Units
    {
        get { return user.units; }
    }

    // Public property to access the user's selected units
    public List<Unit> SelectedUnits
    {
        get { return user.units.FindAll(unit => unit.selected); }
    }


    public UnityEvent OnCurrencyModified = new UnityEvent();
    public UnityEvent OnLevelModified = new UnityEvent();

    // Method to get the singleton instance
    public static GlobalUserData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("GlobalUserData").AddComponent<GlobalUserData>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        if(instance == null) {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }

	public void AddExperience(int experienceToAdd) {
        user.experience += experienceToAdd;

        // Level up
        while (user.experience >= user.experienceToNextLevel) {
            user.experience -= user.experienceToNextLevel;
            user.level++;
            user.experienceToNextLevel = (int)Math.Floor(Math.Pow((float)user.experienceToNextLevel, 1.1));
        }

        OnLevelModified.Invoke();
    }

    public int? GetCurrency(Currency currency) {
        return user.currencies.ContainsKey(currency) ? user.currencies[currency] : null;
    }

    public int GetCurrencyAfkReward(Currency name) {
        return user.accumulatedCurrencyReward.ContainsKey(name) ? user.accumulatedCurrencyReward[name] : 0;
    }

    public int GetMaxCurrencyReward(Currency name) {
        return user.afkMaxCurrencyReward.ContainsKey(name) ? user.afkMaxCurrencyReward[name] : 0;
    }

    public void AddIndividualCurrency(Currency name, int amount) {
        if (user.currencies.ContainsKey(name)) {
            user.currencies[name] = user.currencies[name] + amount;
        } else {
            // User doesn't have this currency.
            if (amount < 0) { throw new InvalidOperationException("AddIndividualCurrency received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with User.CanAfford()"); }
            
            // Create it for him with the given amount.
            user.currencies.Add(name, amount);
        }
        OnCurrencyModified.Invoke();
    }

    public void AddCurrency(Dictionary<Currency, int> currencies) {
        User user = GlobalUserData.Instance.User;

        foreach (var currencyValue in currencies) {
            Currency currency = currencyValue.Key;
            int addAmount = currencyValue.Value;
            
            AddIndividualCurrency(currency, addAmount);
        }
    }

    // This method should be unified with AddIndividualCurrency, only one of these should exist
    public void SetCurrencyAmount(Currency currency, int amount) {
        if (user.currencies.ContainsKey(currency)) {
            user.currencies[currency] = amount;
        } else {
            // User doesn't have this currency.
            if (amount < 0) { throw new InvalidOperationException("SetCurrencyAmount received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with User.CanAfford()"); }
            
            // Create it for him with the given amount.
            user.currencies.Add(currency, amount);
        }
        OnCurrencyModified.Invoke();
    }

	public void ClaimAFKRewards() {
        AccumulateAFKRewards();
        AddCurrency(user.accumulatedCurrencyReward);
        AddExperience(user.accumulatedExperienceReward);
        user.accumulatedCurrencyReward = new Dictionary<Currency, int>();
        user.accumulatedExperienceReward = 0;
    }

	public void AccumulateAFKRewards(){
        DateTime now = DateTime.Now;

        // To cap at 12 hours
        DateTime lastClaim = user.lastAfkRewardClaim;
        TimeSpan maxTimeSpan = new TimeSpan(12, 0, 0);
        // To calculate this interval's rewards
        DateTime lastAccum = user.lastAfkRewardAccum;

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
        int xpToAccum = (int)Math.Round(user.afkMaxExperienceReward * multiplier);
        Dictionary<Currency, int> currencyToAccum = new Dictionary<Currency, int>();
        foreach (var currReward in user.afkMaxCurrencyReward) {
            currencyToAccum.Add(currReward.Key, (int)Math.Round(currReward.Value * multiplier));
        }

        // Update rewards
        user.accumulatedExperienceReward += xpToAccum;
        foreach (var currReward in currencyToAccum) {
            Currency name = currReward.Key;
            int value = currReward.Value;

            if (user.accumulatedCurrencyReward.ContainsKey(name)) {
                user.accumulatedCurrencyReward[name] = user.accumulatedCurrencyReward[name] + value;
            } else {
                user.accumulatedCurrencyReward.Add(name, value);
            }
        }

        user.lastAfkRewardAccum = now;
    }
}
