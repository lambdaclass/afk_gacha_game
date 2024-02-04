using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignsMapManager : MonoBehaviour
{
    public static List<CampaignItem> campaigns;

    void Start()
    {
        SocketConnection.Instance.GetCampaigns();
    }
}
