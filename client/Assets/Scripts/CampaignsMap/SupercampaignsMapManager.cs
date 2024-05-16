using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SupercampaignsMapManager : MonoBehaviour
{
    [SerializeField]
    LevelManager sceneManager;

    [SerializeField]
    AddressableInstantiator addressableInstantiator;

    private GameObject supercampaignInstance;
    public static string selectedSuperCampaignName;

    async void Start()
    {
        await InstantiateSupercampaign();
        SocketConnection.Instance.GetCampaigns(GlobalUserData.Instance.User.id, (campaigns) =>
        {
            // this needs to be refactored, the campaigns have two parallel "paths" that do different things, they should be unified into the static class
            LevelProgress.campaigns = campaigns;
            List<CampaignItem> campaignItems = new List<CampaignItem>(supercampaignInstance.GetComponentsInChildren<CampaignItem>());
            GenerateCampaigns(campaigns, campaignItems);
        });
    }

    private void GenerateCampaigns(List<Campaign> campaigns, List<CampaignItem> campaignItems)
    {
        for (int campaignsIndex = 0; campaignsIndex < campaigns.Count; campaignsIndex++)
        {
            campaignItems[campaignsIndex].sceneManager = sceneManager;
            campaignItems[campaignsIndex].SetCampaignData(campaigns[campaignsIndex]);
        }
    }

    private async Task InstantiateSupercampaign()
    {
        if (selectedSuperCampaignName == "Main")
        {
            supercampaignInstance = await addressableInstantiator.InstantiateMainSupercampaign();
        }
        else
        {
            Debug.LogError("Supercampaign not found: " + selectedSuperCampaignName);
        }
    }
}
