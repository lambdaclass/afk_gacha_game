using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Campaign : MonoBehaviour
{
    [SerializeField]
    GameObject lockObject;

    [SerializeField]
    GameObject completedCrossObject;

    [SerializeField]
    bool first;

    [SerializeField] GameObject campaingToShowPrefab;

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
    public void Select(){
        if (CampaignProgressData.Instance.CampaignStatus(name) == CampaignProgressData.Status.Unlocked) {
            CampaignManager.campaingReference = campaingToShowPrefab;
            SceneManager.LoadScene("Campaign");
        }
    }
}
