using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignsMapManager : MonoBehaviour
{
    [SerializeField]
    private List<CampaignItem> campaignItems;

    [SerializeField]
    SceneManager sceneManager;

    void Start()
    {
        SocketConnection.Instance.GetCampaigns("2123cce2-4a71-4b8d-a95e-d519e5935cc9", (campaigns) => {
            GenerateCampaigns(campaigns);
        });
    }

    private void GenerateCampaigns(List<CampaignData> campaigns)
    {
        // Currently we have 2 campaigns and the client is hardcoded to only manage 2 campaigns, TODO: variable number of campaigns
        for(int campaignsIndex = 0; campaignsIndex < 2; campaignsIndex++) {
            campaignItems[campaignsIndex].sceneManager = sceneManager;
            campaignItems[campaignsIndex].SetCampaignData(campaigns[campaignsIndex]);
        }
    }
}
