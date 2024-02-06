using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignsMapManager : MonoBehaviour
{
    public static List<CampaignData> campaigns;

    void Start()
    {
        SocketConnection.Instance.GetCampaigns();
        StartCoroutine(GenerateCampaigns());
    }

    private IEnumerator GenerateCampaigns()
    {
        yield return new WaitUntil(() => campaigns != null);

        foreach(CampaignData campaign in campaigns) {
            foreach(LevelData level in campaign.levels) {
                Debug.Log($"Level: {level.id}");
                foreach(Unit unit in level.units) {
                    Debug.Log($"{unit.id}");
                }
            }
        }
    }
}
