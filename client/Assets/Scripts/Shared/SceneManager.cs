using System.Collections;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public void ChangeToScene(string sceneName) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
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
