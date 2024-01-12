using System;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

[Serializable]
public class User
{
    public string id { get; set; }

    public string username { get; set; }

    public List<Unit> units { get; set; }

    public int next_unit_id;

    private Dictionary<string, int> currencies = new Dictionary<string, int>()
        {
            { "gold", 100 },
            { "gems", 100 },
            { "scrolls", 10 },
            { "heroic scrolls", 5 }
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
        if (experience > experienceToNextLevel) {
            experience -= experienceToNextLevel;
            level++;
            experienceToNextLevel = (int)Math.Floor(Math.Pow((float)experienceToNextLevel, 1.1));

        }

        OnLevelModified.Invoke();
    }

    public int? GetCurrency(string name) {
        return currencies.ContainsKey(name) ? currencies[name] : null;
    }

    public void AddIndividualCurrency(string name, int ammount) {
        if (currencies.ContainsKey(name)) {
            currencies[name] = currencies[name] + ammount;
        } else {
            // User doesn't have this currency.
            if (ammount < 0) { throw new InvalidOperationException("AddIndividualCurrency received a negative value of a currency the user does not have. This should never happen, otherwise we'd create the currency with a negative value. Possibly an issue with BoxListItem.CanUserBuyItem."); }
            
            // Create it for him with the given amount.
            currencies.Add(name, ammount);
        }
        OnCurrencyModified.Invoke();
    }

    public void AddCurrency(Dictionary<string, int> currencies) {
        User user = GlobalUserData.Instance.User;

        foreach (var currencyValue in currencies) {
            string currency = currencyValue.Key;
            int addAmount = currencyValue.Value;
            
            user.AddIndividualCurrency(currency, addAmount);
        }
    }

    public void SubstractCurrency(Dictionary<string, int> currencies) {
        Dictionary<string, int> negativeCurrencies = new Dictionary<string, int>();

        foreach (var pair in currencies)
        {
            negativeCurrencies.Add(pair.Key, -pair.Value);
        }

        AddCurrency(negativeCurrencies);
    }
}
