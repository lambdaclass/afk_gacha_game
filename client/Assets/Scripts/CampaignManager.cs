using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static GameObject campaignPrefab;
    
    public static Campaign selectedCampaignData;
    
    public static string automaticLoadLevelName = null;
    
    [SerializeField]
    GameObject screenLocker;

    void Start() {
        BattleManager.selectedLevelData = null;
		GameObject campaignGameObject;
		if(campaignPrefab != null) {
        	campaignGameObject = Instantiate(campaignPrefab, transform);
		}
		if(campaignGameObject == null) {
			Debug.LogError("campaignGameObject == null");
		}
        campaignGameObject.transform.SetSiblingIndex(0);
        var campaignLevelManager = campaignGameObject.GetComponentInChildren<CampaignLevelsManager>();
        campaignLevelManager.AssignLevelsData(selectedCampaignData.levels);

        if (automaticLoadLevelName != null) {
            CampaignLevelIndicator level = campaignGameObject.transform.Find("CampaignLevelManager").transform.Find(automaticLoadLevelName).gameObject.GetComponent<CampaignLevelIndicator>();
            level.SelectLevel();
            screenLocker.SetActive(true);
            StartCoroutine(gameObject.GetComponent<LevelManager>().ChangeToSceneAfterSeconds("Lineup", 1));
            automaticLoadLevelName = null;
        }
    }
}
