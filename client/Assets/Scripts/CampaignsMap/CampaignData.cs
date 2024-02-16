using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignData
{
    public Status status = Status.Locked;
    public List<LevelData> levels;

    public enum Status
    {
        Locked,
        Unlocked,
        Completed
    }
}
