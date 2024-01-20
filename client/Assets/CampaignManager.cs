using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static GameObject campaignReference;

    public static string automaticLoadLevelName = null;
    [SerializeField] GameObject screenLocker;

    void Start() {
        LevelData.Instance.Level = null;
        GameObject campaign = Instantiate(campaignReference, transform);
        campaign.transform.SetSiblingIndex(0);
        
        if (automaticLoadLevelName != null) {
            Level level = campaign.transform.Find("CampaignLevelManager").transform.Find(automaticLoadLevelName).gameObject.GetComponent<Level>();
            level.SelectLevel();
            screenLocker.SetActive(true);
            StartCoroutine(gameObject.GetComponent<LevelManager>().ChangeToSceneAfterSeconds("Lineup", 1));
            automaticLoadLevelName = null;
        }
    }
}
