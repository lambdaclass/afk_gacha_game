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

    public void ModifyCurrency(string name, int ammount) {
        if (currencies.ContainsKey(name)) {
            currencies[name] = currencies[name] + ammount;
            OnCurrencyModified.Invoke();
        } else {
            Debug.LogError("Currency " + name + " not found amongst user's currencies.");
        }
    }
}
