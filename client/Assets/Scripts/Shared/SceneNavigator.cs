using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneNavigator : MonoBehaviour
{
    public static Stack<string> sceneHistory = new Stack<string>();

    public void ChangeToScene(string sceneName) {
		sceneHistory.Push(SceneManager.GetActiveScene().name); 
        SceneManager.LoadScene(sceneName);
    }

	public void BackToPreviousScene() {
		SceneManager.LoadScene(sceneHistory.Pop());
	}

    // This is a workaround to avoid killing the sound effect when changing scenes.
    public void ChangeToSceneWithDelay(string sceneName) {
        StartCoroutine(ChangeToSceneAfterSeconds(sceneName, 0.1f));
    }

    public IEnumerator ChangeToSceneAfterSeconds(string sceneName, float seconds) {
        yield return new WaitForSeconds(seconds);
        ChangeToScene(sceneName);
    }
}
