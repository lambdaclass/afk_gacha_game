using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbiesManager : LevelSelector
{
    [SerializeField]
    Text sessionId;

    public override void GoToLevel()
    {
        base.GoToLevel();
    }

    public void CreateLobby()
    {
        LobbyConnection.Instance.CreateLobby();
        GoToLevel();
    }

    public void ConnectToLobby()
    {
        LobbyConnection.Instance.ConnectToLobby(sessionId.text);
        GoToLevel();
    }

    public void Back()
    {
        LobbyConnection.Instance.Init();
        GoToLevel();
    }
}
