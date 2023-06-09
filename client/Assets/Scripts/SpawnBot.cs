using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class SpawnBot : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] SocketConnectionManager manager;

    private bool pendingSpawn = false;
    private string botId;

    public static SpawnBot Instance;

    public void Init()
    {
        if (manager.players.Count == 9) GetComponent<MMTouchButton>().DisableButton();
        Instance = this;
        GenerateBotPlayer();
    }

    public void GenerateBotPlayer()
    {
        manager.CallSpawnBot();
    }

    public void Spawn(string Id)
    {
        pendingSpawn = true;
        botId = Id;
    }

    public void Update()
    {
        print(botId);
        if (pendingSpawn)
        {
            playerPrefab.GetComponent<Character>().PlayerID = "";

            Character newPlayer = Instantiate(
                playerPrefab.GetComponent<Character>(),
                new Vector3(0, 0, 0),
                Quaternion.identity
            );
            newPlayer.PlayerID = botId.ToString();
            newPlayer.name = "BOT" + botId;
            manager.players.Add(newPlayer.gameObject);
            print("SPAWNED");

            pendingSpawn = false;
        }
    }
}
