using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CampaignLevelIndicator : MonoBehaviour
{
    public LevelData levelData;

    [SerializeField]
	GameObject lockObject;

    [SerializeField]
	GameObject completedCrossObject;

    public void Init() {
        if(levelData.status == LevelProgressData.Status.Unlocked) {
			LevelProgressData.Instance.SetUnlocked(name);
		}

        switch(levelData.status) 
        {
            case LevelProgressData.Status.Locked:
				lockObject.SetActive(true);
                break;
            case LevelProgressData.Status.Unlocked:
                break;
            case LevelProgressData.Status.Completed:
                completedCrossObject.SetActive(true);
                break;
        }
    }

    public void SelectLevel(){
        if(LevelProgressData.Instance.LevelStatus(name) != LevelProgressData.Status.Unlocked) {
            return;
        }

        // Get the CampaignLevelManager parent and make it pop up the Battle button
        Transform parent = transform.parent;
        if (parent.TryGetComponent<CampaignLevelsManager>(out CampaignLevelsManager campaignLevelManager)) {
			campaignLevelManager.LevelSelected();
		}
        else {
			Debug.LogError("Level has no CampaignLevelManager parent.");
		}
        
        SetLevel();
        SetLevelToComplete();
        SetLevelToUnlock();
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
}
