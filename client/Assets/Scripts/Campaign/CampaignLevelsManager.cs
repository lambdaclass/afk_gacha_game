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
        // Get the min between the levels from the backend and the level indicators from the scene to assign to not go OutOfRange
        for(int levelIndex = 0; levelIndex < Math.Min(levelsData.Count, levelIndicators.Count); levelIndex++) {
            levelIndicators[levelIndex].levelData = levelsData[levelIndex];
            levelIndicators[levelIndex].Init();
        }
    }
}
