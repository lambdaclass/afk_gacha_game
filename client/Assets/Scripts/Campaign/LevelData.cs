using System.Collections;
using System.Collections.Generic;

public class LevelData
{
    // This fields shouldn't be modifiable externally (they shouldn't be public?)
    public string id;
    public int levelNumber;
    public int campaign;
    public List<Unit> units;

    // Hardcoded values aren't currently brought from the backend

    public Dictionary<Currency, int> rewards = new Dictionary<Currency, int>(){
        { Currency.Gold, 1000 },
        { Currency.Gems, 45 },
        { Currency.Scrolls, 2 }
    };

    // AFK Rewards rate granted
    // These are how many a player makes in the maximum timespan available (12h now)
    public Dictionary<Currency, int> afkCurrencyRate = new Dictionary<Currency, int>() {
        { Currency.Gold, 270 }
    };

    public int afkExperienceRate;

    public int experienceReward;

    // Wether this level is the first one of the campaign (unlocked automatically)
    public bool first;
}
