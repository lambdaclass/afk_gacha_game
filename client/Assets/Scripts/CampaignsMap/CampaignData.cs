using System.Collections.Generic;

public class Campaign
{
	public string campaignId;
	public int campaignNumber;
    public LevelProgress.Status status = LevelProgress.Status.Locked;
    public List<LevelData> levels;
}
