using UnityEngine;
using System.Collections.Generic;
using System;

public class CampaignLevelsManager : MonoBehaviour
{
    [SerializeField]
    GameObject PlayButton;

    [SerializeField]
    List<CampaignLevelIndicator> levelIndicators;

    public void LevelSelected() {
        PlayButton.SetActive(true);
    }

    public void AssignLevelsData(List<LevelData> levelsData)
    {
        for(int levelIndex = 0; levelIndex < levelsData.Count; levelIndex++) {
            levelIndicators[levelIndex].levelData = levelsData[levelIndex];
        }
    }
}
