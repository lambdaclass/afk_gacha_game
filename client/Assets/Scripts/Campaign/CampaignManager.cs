using UnityEngine;

public class CampaignManager : MonoBehaviour
{
    public static GameObject campaignPrefab;

    public static Campaign selectedCampaignData;

    [SerializeField]
    GameObject screenLocker;

    [SerializeField]
    SceneNavigator sceneNavigator;

    void Start()
    {
        LevelProgress.selectedLevelData = null;
        LevelProgress.nextLevelData = null;
        GameObject campaignGameObject = Instantiate(campaignPrefab, transform);
        campaignGameObject.transform.SetSiblingIndex(0);
        var campaignLevelManager = campaignGameObject.GetComponentInChildren<CampaignLevelsManager>();
        campaignLevelManager.AssignLevelsData(selectedCampaignData.levels, sceneNavigator);
    }
}
