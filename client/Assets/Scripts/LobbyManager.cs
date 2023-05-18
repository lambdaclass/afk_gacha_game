using System;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class LobbyManager : LevelSelector
{
    [SerializeField]
    GameObject playButton;

    public override void GoToLevel()
    {
        base.GoToLevel();
        gameObject.GetComponent<MMTouchButton>().DisableButton();
    }

    void Start()
    {
        if (LobbyConnection.Instance.playerId == 1)
        {
            playButton.SetActive(true);
        }
        else
        {
            playButton.SetActive(false);
        }
    }

    public void GameStart()
    {
        LobbyConnection.Instance.StartGame();
        GoToLevel();
    }

    public void Back()
    {
        LobbyConnection.Instance.Init();
        GoToLevel();
    }

    private void Update()
    {
        if (
            !String.IsNullOrEmpty(LobbyConnection.Instance.GameSession)
            && LobbyConnection.Instance.gameStarted == false
        )
        {
            LobbyConnection.Instance.StartGame();
            MMSceneLoadingManager.LoadScene("BackendPlayground");
        }
    }
}
