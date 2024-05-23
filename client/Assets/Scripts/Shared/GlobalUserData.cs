using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GlobalUserData : MonoBehaviour
{
    // This should be in it's own manager like of Champions of Mirra, not a list in the user singleton (should there be a user singleton?)
    [SerializeField]
    List<Character> characters;

    public List<Character> AvailableCharacters
    {
        get
        {
            return this.characters;
        }
    }

    [SerializeField]
    List<ItemTemplate> itemtemplates;
    public List<ItemTemplate> AvailableItemTemplates
    {
        get
        {
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
        set
        {
            user = value;
            OnChangeUser.Invoke();
        }
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

    public UnityEvent OnChangeUser = new UnityEvent();
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
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }

    private void AddExperience(int experienceToAdd)
    {
        user.experience += experienceToAdd;

        // Level up
        while (user.experience >= user.experienceToNextLevel)
        {
            user.experience -= user.experienceToNextLevel;
            user.level++;
            user.experienceToNextLevel = (int)Math.Floor(Math.Pow((float)user.experienceToNextLevel, 1.1));
        }

        OnLevelModified.Invoke();
    }

    public int? GetCurrency(Currency currency)
    {
        return user.currencies.ContainsKey(currency) ? user.currencies[currency] : null;
    }

    public int GetCurrencyAfkReward(Currency name)
    {
        return user.accumulatedCurrencyReward.ContainsKey(name) ? user.accumulatedCurrencyReward[name] : 0;
    }

    public int GetMaxCurrencyReward(Currency name)
    {
        return user.afkMaxCurrencyReward.ContainsKey(name) ? user.afkMaxCurrencyReward[name] : 0;
    }

    public void AddCurrency(Currency name, int amount)
    {
        if (name == Currency.Experience)
        {
            AddExperience(amount);
            return;
        }

        if (user.currencies.ContainsKey(name))
        {
            user.currencies[name] = user.currencies[name] + amount;
        }
        else
        {
            // User doesn't have this currency.
            if (amount < 0) { throw new InvalidOperationException("AddCurrency received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with User.CanAfford()"); }

            // Create it for him with the given amount.
            user.currencies.Add(name, amount);
        }
        OnCurrencyModified.Invoke();
    }

    public void AddCurrencies(Dictionary<Currency, int> currencies)
    {
        User user = GlobalUserData.Instance.User;

        foreach (var currencyValue in currencies)
        {
            Currency currency = currencyValue.Key;
            int addAmount = currencyValue.Value;

            AddCurrency(currency, addAmount);
        }
    }

    // This method should be unified with AddCurrency, only one of these should exist
    public void SetCurrencyAmount(Currency currency, int amount)
    {
        if (user.currencies.ContainsKey(currency))
        {
            user.currencies[currency] = amount;
        }
        else
        {
            // User doesn't have this currency.
            if (amount < 0) { throw new InvalidOperationException("SetCurrencyAmount received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with User.CanAfford()"); }

            // Create it for him with the given amount.
            user.currencies.Add(currency, amount);
        }
        OnCurrencyModified.Invoke();
    }
}
