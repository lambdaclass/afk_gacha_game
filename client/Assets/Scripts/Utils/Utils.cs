using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Utils
{
    public static IEnumerator WaitForGameCreation()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(LobbyConnection.Instance.GameSession));
        SceneManager.LoadScene("BackendPlayground");
    }
}
