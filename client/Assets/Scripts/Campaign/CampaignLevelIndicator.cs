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

	[SerializeField]
	CampaignLevelsManager campaignLevelManager;

    public void Init() {
        if(levelData.status == LevelProgressData.Status.Unlocked) {
			LevelProgressData.Instance.SetUnlocked(name);
		}

        switch(levelData.status) 
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
		campaignLevelManager.LevelSelected();
        
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
