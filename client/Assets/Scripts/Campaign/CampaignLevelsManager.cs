using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CampaignLevelsManager : MonoBehaviour
{
    [SerializeField]
    GameObject playButton;

    [SerializeField]
    List<CampaignLevelIndicator> levelIndicators;

    [SerializeField]
    GameObject insufficientCurrenciesPopup;


    public void LevelSelected()
    {
        playButton.SetActive(true);
    }

    public void AssignLevelsData(List<LevelData> levelsData, SceneNavigator sceneNavigator)
    {
        // The campaign prefabs should match the number of levels with the levels brought with the backend, if not a warning will be shown.
        int levelsInScene = Math.Min(levelsData.Count, levelIndicators.Count);
        if (levelsData.Count != levelIndicators.Count)
        {
            Debug.LogWarning("The number of levels brought from the backend doesn't match with the number of levels on that campaign prefab.");
        }

        for (int levelIndex = 1; levelIndex < levelsInScene + 1; levelIndex++)
        {
            levelIndicators[levelIndex - 1].levelData = levelsData[levelIndex - 1];

            if (levelIndex < levelsData.Count)
            {
                levelIndicators[levelIndex - 1].nextLevelData = levelsData[levelIndex];
            }
            else
            {
                levelIndicators[levelIndex - 1].nextLevelData = LevelProgress.NextLevel(levelsData[levelIndex - 1]);
            }

            levelIndicators[levelIndex - 1].Init();
        }

        playButton.GetComponent<Button>().onClick.AddListener(() => sceneNavigator.ChangeToScene("Lineup"));
    }
}
