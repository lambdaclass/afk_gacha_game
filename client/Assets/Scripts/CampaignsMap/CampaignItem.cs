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
    bool first;

    [SerializeField]
    GameObject campaignToShowPrefab;

    public SceneManager sceneManager;

    private void Start(){
        if(first) { CampaignProgressData.Instance.SetUnlocked(name); }

        switch(CampaignProgressData.Instance.CampaignStatus(name)) {
            case CampaignProgressData.Status.Locked:
                break;
            case CampaignProgressData.Status.Unlocked:
                lockObject.SetActive(false);
                break;
            case CampaignProgressData.Status.Completed:
                completedCrossObject.SetActive(true);
                lockObject.SetActive(false);
                break;
        }
    }

    // Load campaign scene if its unlocked. Scene needs to have the same name as our campaign object.
    public void SelectCampaign(CampaignData campaignData)
    {
        if (CampaignProgressData.Instance.CampaignStatus(name) == CampaignProgressData.Status.Unlocked) {
            CampaignManager.selectedCampaignData = campaignData;
            CampaignManager.campaignPrefab = campaignToShowPrefab;
            sceneManager.ChangeToSceneWithDelay("Campaign");
        }
    }

    public void SetCampaignData(CampaignData campaignData)
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => SelectCampaign(campaignData));
    }
}
