using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;

public class ConnectionUtils
{
    public static IEnumerator WaitForGameCreation()
    {
        yield return new WaitUntil(() => !string.IsNullOrEmpty(LobbyConnection.Instance.GameSession));
        LobbyConnection.Instance.playerCount = 1;
        SceneManager.LoadScene("BackendPlayground");
    }
}
