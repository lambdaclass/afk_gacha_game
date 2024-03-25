using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Campaign
{
	public string campaignId;
    public LevelProgressData.Status status = LevelProgressData.Status.Locked;
    public List<LevelData> levels;
}
