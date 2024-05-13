using System.Collections.Generic;
using UnityEngine;

public class SupercampaignsMapManager : MonoBehaviour
{
    [SerializeField]
    LevelManager sceneManager;
    private List<CampaignItem> campaignItems;
    private GameObject supercampaignToShowPrefab;
    public static string selectedSuperCampaignName;

    void Start()
    {
        Debug.Log("Selected supercampaign: " + selectedSuperCampaignName);

        supercampaignToShowPrefab = Object.Instantiate(Resources.Load<GameObject>("Prefabs/Supercampaigns/" + selectedSuperCampaignName));
        SocketConnection.Instance.GetCampaigns(GlobalUserData.Instance.User.id, (campaigns) =>
        {
            // this needs to be refactored, the campaigns have two parallel "paths" that do different things, they should be unified into the static class
            LevelProgress.campaigns = campaigns;
            GenerateCampaigns(campaigns);
        });
    }

    private void GenerateCampaigns(List<Campaign> campaigns)
    {
        // Currently we have 2 campaigns and the client is hardcoded to only manage 2 campaigns, TODO: variable number of campaigns
        for (int campaignsIndex = 0; campaignsIndex < 2; campaignsIndex++)
        {
            campaignItems[campaignsIndex].sceneManager = sceneManager;
            campaignItems[campaignsIndex].SetCampaignData(campaigns[campaignsIndex]);
        }
    }
}
