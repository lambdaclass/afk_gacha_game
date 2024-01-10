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

    private void Start(){
        if(first) { CampaignProgressData.Instance.SetUnlocked(name); }

        print(name);
        print(CampaignProgressData.Instance.CampaignStatus(name));
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
            SceneManager.LoadScene(name);
        }
    }
}
