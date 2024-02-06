using System.Collections;
using System.Collections.Generic;

public class LevelData
{
    public string id;
    public int levelNumber;
    public int campaign;
    public List<Unit> units;

    // Currency rewards
    public CurrencyValue[] individualRewards;
    public Dictionary<Currency, int> rewards = new Dictionary<Currency, int>();

    public int experienceReward;

    // Unlock this level if current level is beaten.
    // Level instead of string (like campaigns) to make it easier to set up in UI.
    public CampaignLevelIndicator nextLevel;

    // Wether this level is the first one of the campaign (unlocked automatically)
    public bool first;

    // Mark this campaign as completed if this level is beaten
    public string campaignToComplete;

    // Unlock this campaign if this level is beaten
    public string campaignToUnlock;

    // AFK Rewards rate granted
    // These are how many a player makes in the maximum timespan available (12h now)
    public CurrencyValue[] individualAfkCurrencyRates;
    public Dictionary<Currency, int> afkCurrencyRate = new Dictionary<Currency, int>();

    public int afkExperienceRate;
}
