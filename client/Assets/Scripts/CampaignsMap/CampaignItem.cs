using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CampaignItem : MonoBehaviour
{
    [SerializeField]
    GameObject lockObject;

    [SerializeField]
    GameObject completedCrossObject;

    [SerializeField]
    GameObject campaignToShowPrefab;

    private CampaignData campaignData;

    public LevelManager sceneManager;

    // Load campaign scene if its unlocked. Scene needs to have the same name as our campaign object.
    public void SelectCampaign()
    {
        if (campaignData.status == CampaignData.Status.Unlocked) {
            CampaignManager.selectedCampaignData = campaignData;
            CampaignManager.campaignPrefab = campaignToShowPrefab;
            sceneManager.ChangeToSceneWithDelay("Campaign");
        }
    }

    public void SetCampaignData(CampaignData campaignData)
    {
        this.campaignData = campaignData;
        if(campaignData.status == CampaignData.Status.Unlocked) {
            lockObject.SetActive(false);
        }
        gameObject.GetComponent<Button>().onClick.AddListener(() => SelectCampaign());
    }
}
