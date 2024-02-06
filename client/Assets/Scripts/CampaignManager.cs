using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static GameObject campaignPrefab;
    
    public static CampaignData selectedCampaignData;
    
    public static string automaticLoadLevelName = null;
    
    [SerializeField]
    GameObject screenLocker;

    void Start() {
        BattleManager.selectedLevelData = null;
        GameObject campaignGameObject = Instantiate(campaignPrefab, transform);
        campaignGameObject.transform.SetSiblingIndex(0);
        campaignGameObject.GetComponent<CampaignLevelsManager>().AssignLevelsData(selectedCampaignData.levels);

        if (automaticLoadLevelName != null) {
            CampaignLevelIndicator level = campaignGameObject.transform.Find("CampaignLevelManager").transform.Find(automaticLoadLevelName).gameObject.GetComponent<CampaignLevelIndicator>();
            level.SelectLevel();
            screenLocker.SetActive(true);
            StartCoroutine(gameObject.GetComponent<SceneManager>().ChangeToSceneAfterSeconds("Lineup", 1));
            automaticLoadLevelName = null;
        }
    }
}
