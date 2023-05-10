using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class LobbyManager : LevelSelector
{
    public LobbyConnection lobbyConnection;
    public override void GoToLevel()
    {
        base.GoToLevel();
        // lobbyConnection.CreateLobby();
        lobbyConnection.Init();
        print("LOBBy" + lobbyConnection.lobbiesList.Count);
    }
}
