using UnityEngine;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

public class CampaignLevelsManager : MonoBehaviour
{
	[SerializeField]
	GameObject playButton;

	[SerializeField]
	List<CampaignLevelIndicator> levelIndicators;

	public void LevelSelected()
	{
		playButton.SetActive(true);
	}

	public void AssignLevelsData(List<LevelData> levelsData, SceneNavigator sceneNavigator)
	{
		// The campaign prefabs need to match the number of levels with the levels brought with the backend, if not this will break.
		if (levelsData.Count != levelIndicators.Count)
		{
			throw new Exception("The number of levels brought from the backend doesn't match with the number of levels on that campaign prefab.");
		}

		for (int levelIndex = 1; levelIndex < levelsData.Count + 1; levelIndex++)
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
