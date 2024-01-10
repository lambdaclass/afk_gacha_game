using UnityEngine;
using System.Collections.Generic;

public class CampaignProgressData : MonoBehaviour
{
    private static CampaignProgressData instance;

    // Dictionary to store the locked/unlocked state of each campaign
    private readonly Dictionary<string, Status> campaignStates = new Dictionary<string, Status>();

    private string campaignToComplete;
    private string campaignToUnlock;

    public enum Status
    {
        Locked,
        Unlocked,
        Completed
    }

    // Public property to set the next campaign
    public string CampaignToComplete
    {
        get { return campaignToComplete; }
        set { campaignToComplete = value; }
    }

    // Public property to set the next campaign
    public string CampaignToUnlock
    {
        get { return campaignToUnlock; }
        set { campaignToUnlock = value; }
    }

    // Singleton instance
    public static CampaignProgressData Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("CampaignProgressData").AddComponent<CampaignProgressData>();
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
        } else {
            // Destroy this instance if another one already exists
            Destroy(gameObject);
        }
    }

    // Method to check the status of a campaign
    public Status CampaignStatus(string campaignName)
    {
        return campaignStates.ContainsKey(campaignName) ? campaignStates[campaignName] : Status.Locked;

    }

    // Called on battles won.
    public void ProcessLevelCompleted()
    {
        if(campaignToComplete != null) {
        // Key should exist, this is just to be sure
            if (campaignStates.ContainsKey(campaignToComplete))
            {
                campaignStates[campaignToComplete] = Status.Completed;
            }
            else
            {
                campaignStates.Add(campaignToComplete, Status.Completed);
            }
        }

        if(campaignToUnlock != null) {
            // Key should not exist, this is just to be sure
            if (campaignStates.ContainsKey(campaignToUnlock))
            {
                campaignStates[campaignToUnlock] = Status.Unlocked;
            }
            else
            {
                campaignStates.Add(campaignToUnlock, Status.Unlocked);
            }
        }
    }

    // To be called with campaigns initially unlocked. Doesn't modify data if it already exists.
    public void SetUnlocked(string campaignName) {
        if(!campaignStates.ContainsKey(campaignName)) { campaignStates.Add(campaignName, Status.Unlocked); }
    }
}
