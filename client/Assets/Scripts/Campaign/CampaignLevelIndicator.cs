using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignLevelIndicator : MonoBehaviour
{
    public LevelData levelData;

    [SerializeField] GameObject lockObject;

    [SerializeField] GameObject completedCrossObject;

    public void Init() {
        if(levelData.first) {
			LevelProgressData.Instance.SetUnlocked(name);
		}

        switch(LevelProgressData.Instance.LevelStatus(name)) 
        {
            case LevelProgressData.Status.Locked:
                break;
            case LevelProgressData.Status.Unlocked:
                lockObject.SetActive(false);
                break;
            case LevelProgressData.Status.Completed:
                completedCrossObject.SetActive(true);
                lockObject.SetActive(false);
                break;
        }
    }

    public void SelectLevel(){
        if(LevelProgressData.Instance.LevelStatus(name) != LevelProgressData.Status.Unlocked) {
            return;
        }

        // Get the CampaignLevelManager parent and make it pop up the Battle button
        Transform parent = transform.parent;
        if (parent.TryGetComponent<CampaignLevelsManager>(out CampaignLevelsManager campaignLevelManager)) { campaignLevelManager.LevelSelected(); }
        else { Debug.LogError("Level has no CampaignLevelManager parent."); }
        
        SetLevel();
        SetLevelToComplete();
        SetLevelToUnlock();
        SetCampaignToComplete();
        SetCampaignToUnlock();
    }

    private void SetLevel() {
        BattleManager.selectedLevelData = levelData;
    }

    private void SetLevelToComplete() {
        LevelProgressData.Instance.LevelToCompleteName = name;
    }

    private void SetLevelToUnlock() {
        // if(levelData.nextLevel != null) { LevelProgressData.Instance.LevelToUnlockName = levelData.nextLevel.name; }
        // else { LevelProgressData.Instance.LevelToUnlockName = null; }

        LevelProgressData.Instance.LevelToUnlockName = null;
    }

    private void SetCampaignToComplete() {
        // CampaignProgressData.Instance.CampaignToComplete = levelData.campaignToComplete;
    }

    private void SetCampaignToUnlock() {
        // CampaignProgressData.Instance.CampaignToUnlock = levelData.campaignToUnlock;
    }
}
