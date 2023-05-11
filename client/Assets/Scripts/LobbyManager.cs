using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : LevelSelector
{
    public override void GoToLevel()
    {
        base.GoToLevel();
        if (SceneManager.GetActiveScene().name == "Lobbies")
        {
            LobbyConnection.Instance.Init();
        }
        else
        {
            if (LobbyConnection.Instance.playerId == 1)
            {
                LobbyConnection.Instance.StartGame();
            }
        }
    }
}
