using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignsMapManager : MonoBehaviour
{
    [NonSerialized]
    public static List<CampaignData> campaigns;

    [SerializeField]
    private List<CampaignItem> campaignItems;

    [SerializeField]
    SceneManager sceneManager;

    void Start()
    {
        SocketConnection.Instance.GetCampaigns();
        StartCoroutine(GenerateCampaigns());
    }

    private IEnumerator GenerateCampaigns()
    {
        yield return new WaitUntil(() => campaigns != null);

        // Currently we have 2 campaigns and the client is hardcoded to only manage 2 campaigns, TODO: variable number of campaigns
        for(int campaignsIndex = 0; campaignsIndex < 2; campaignsIndex++) {
            campaignItems[campaignsIndex].sceneManager = sceneManager;
            campaignItems[campaignsIndex].SetCampaignData(campaigns[campaignsIndex]);
        }
    }
}
