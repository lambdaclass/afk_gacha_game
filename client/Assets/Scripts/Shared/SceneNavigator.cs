using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
	public static Stack<string> sceneHistory = new Stack<string>();

	void Update()
	{
		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
		{
			if (Input.GetKey(KeyCode.Escape))
			{
				BackToPreviousScene();
			}
		}
	}

	public void ChangeToScene(string targetSceneName)
	{

		if (sceneHistory.Contains(targetSceneName))
		{
			while (targetSceneName != sceneHistory.Peek())
			{
				sceneHistory.Pop();
			}
			sceneHistory.Pop();
		}
		else
		{
			sceneHistory.Push(SceneManager.GetActiveScene().name);
		}

		SceneManager.LoadScene(targetSceneName);
	}

	public void BackToPreviousScene()
	{
		SceneManager.LoadScene(sceneHistory.Pop());
	}

	// This is a workaround to avoid killing the sound effect when changing scenes.
	public void ChangeToSceneWithDelay(string sceneName)
	{
		StartCoroutine(ChangeToSceneAfterSeconds(sceneName, 0.1f));
	}

	public IEnumerator ChangeToSceneAfterSeconds(string sceneName, float seconds)
	{
		yield return new WaitForSeconds(seconds);
		ChangeToScene(sceneName);
	}
}
