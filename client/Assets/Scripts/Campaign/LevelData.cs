using System.Collections;
using System.Collections.Generic;

public class LevelData
{
    // This fields shouldn't be modifiable externally (they shouldn't be public?)
    public string id;
    public int levelNumber;
    public string campaignId;
    public List<Unit> units;

    public Dictionary<string, int> currencyRewards = new Dictionary<string, int>();

    public List<ItemReward> itemRewards = new();
    public List<UnitReward> unitRewards = new();

    public int experienceReward;

    public LevelProgress.Status status;
}
