using System.Collections.Generic;
using UnityEngine;

public class SupercampaignsMapManager : MonoBehaviour
{
    [SerializeField]
    LevelManager sceneManager;
    private GameObject supercampaignPrefab;
    public static string selectedSuperCampaignName;

    void Start()
    {
        GameObject selectedPrefab = Resources.Load("Supercampaigns/" + selectedSuperCampaignName) as GameObject;
        supercampaignPrefab = Instantiate(selectedPrefab, new Vector3(0, 0, 0), Quaternion.identity);

        SocketConnection.Instance.GetCampaigns(GlobalUserData.Instance.User.id, (campaigns) =>
        {
            // this needs to be refactored, the campaigns have two parallel "paths" that do different things, they should be unified into the static class
            LevelProgress.campaigns = campaigns;
            List<CampaignItem> campaignItems = new List<CampaignItem>(supercampaignPrefab.GetComponentsInChildren<CampaignItem>());
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
}
