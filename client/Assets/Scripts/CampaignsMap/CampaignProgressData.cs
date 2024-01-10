using UnityEngine;
using System.Collections.Generic;

public class CampaignProgressData : MonoBehaviour
{
    private static CampaignProgressData instance;

    // Dictionary to store the locked/unlocked state of each campaign
    private readonly Dictionary<string, bool> campaignStates = new Dictionary<string, bool>();

    private string campaignToUnlockName;

    // Public property to set the next campaign
    public string CampaignToUnlockName
    {
        set { campaignToUnlockName = value; }
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

    // Method to check if a campaign is locked
    public bool IsCampaignUnlocked(string campaignName)
    {
        return campaignStates.ContainsKey(campaignName) ? campaignStates[campaignName] : false;
    }

    // Method to unlock a campaign
    public void UnlockNextCampaign()
    {
        if (campaignStates.ContainsKey(campaignToUnlockName))
        {
            campaignStates[campaignToUnlockName] = true;
        }
        else
        {
            campaignStates.Add(campaignToUnlockName, true);
        }
    }
}
