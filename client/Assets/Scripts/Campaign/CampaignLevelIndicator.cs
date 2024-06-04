using UnityEngine;
using UnityEngine.UI;

public class CampaignLevelIndicator : MonoBehaviour
{
    public LevelData levelData;
    public LevelData nextLevelData;

    [SerializeField]
    GameObject lockObject;

    [SerializeField]
    GameObject completedCrossObject;

    [SerializeField]
    CampaignLevelsManager campaignLevelManager;

    public void Init()
    {
        switch (levelData.status)
        {
            case LevelProgress.Status.Locked:
                break;
            case LevelProgress.Status.Unlocked:
                lockObject.SetActive(false);
                GetComponent<Button>().enabled = true;
                break;
            case LevelProgress.Status.Completed:
                completedCrossObject.SetActive(true);
                lockObject.SetActive(false);
                break;
        }
    }

    public void SelectLevel()
    {
        SetLevel();
        LineupManager.LevelData = levelData;
        campaignLevelManager.LevelSelected();
    }

    private void SetLevel()
    {

        LevelProgress.selectedLevelData = levelData;
        LevelProgress.nextLevelData = nextLevelData;
    }
}
