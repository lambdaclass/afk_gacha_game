using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : LevelSelector
{
    [SerializeField]
    GameObject playButton;

    public override void GoToLevel()
    {
        base.GoToLevel();
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
        if (!String.IsNullOrEmpty(LobbyConnection.Instance.GameSession) && LobbyConnection.Instance.gameStarted == false)
        {
            LobbyConnection.Instance.StartGame();
            MMSceneLoadingManager.LoadScene("BackendPlayground");
        }
    }
}
