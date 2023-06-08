using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;
using UnityEngine;
using UnityEngine.Events;

public class CustomLevelManager : LevelManager
{
    private List<Player> gamePlayers;
    private int totalPlayers;
    private int playerId;
    public Character prefab;
    public Camera UiCamera;
    public CinemachineCameraController camera;

    bool paused = false;

    protected override void Awake()
    {
        base.Awake();
        this.totalPlayers = LobbyConnection.Instance.playerCount;
    }

    protected override void Start()
    {
        base.Start();
        StartCoroutine(InitializeLevel());
    }

    // TODO: rename this method
    private IEnumerator InitializeLevel()
    {
        yield return new WaitUntil(() => SocketConnectionManager.Instance.gamePlayers != null);
        this.gamePlayers = SocketConnectionManager.Instance.gamePlayers;
        GeneratePlayer();
        playerId = LobbyConnection.Instance.playerId;
        setCameraToPlayer(playerId);
        SetInputsAbilities(playerId);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GUIManager.Instance.SetPauseScreen(paused == false ? true : false);
            paused = !paused;
        }
    }

    public void GeneratePlayer()
    {
        for (int i = 0; i < totalPlayers; i++)
        {
            if (LobbyConnection.Instance.playerId == i + 1)
            {
                // Player1 is the ID to match with the client InputManager
                prefab.PlayerID = "Player1";
            }
            else
            {
                prefab.PlayerID = "";
            }
            Character newPlayer = Instantiate(
                prefab,
                // The following is a transformation from the back-end's coordinate system to the front-end's.
                new Vector3(
                    ((((float)gamePlayers[i].Position.Y - 500) / 10)),
                    1.04f,
                    ((float)(((float)gamePlayers[i].Position.X - 500) / 10)) * (-1)
                ),
                Quaternion.identity
            );
            newPlayer.name = "Player" + " " + (i + 1);
            newPlayer.PlayerID = (i + 1).ToString();

            SocketConnectionManager.Instance.players.Add(newPlayer.gameObject);
            this.Players.Add(newPlayer);
        }
        this.PlayerPrefabs = (this.Players).ToArray();
    }

    private void setCameraToPlayer(int playerID)
    {
        foreach (Character player in this.PlayerPrefabs)
        {
            if (Int32.Parse(player.PlayerID) == playerID)
            {
                this.camera.SetTarget(player);
                this.camera.StartFollowing();
            }
        }
    }

    private void SetInputsAbilities(int playerID)
    {
        foreach (Character player in this.PlayerPrefabs)
        {
            if (Int32.Parse(player.PlayerID) == playerID)
            {
                UnityEvent aoeEvent = new UnityEvent();
                aoeEvent.AddListener(player.GetComponent<GenericAoeAttack>().ShowAimAoeAttack);
                UiCamera
                    .GetComponent<CustomInputManager>()
                    .AssignInputToAbilityPosition("y", "joystick", aoeEvent);

                UnityEvent<Vector2> aimEvent = new UnityEvent<Vector2>();
                aimEvent.AddListener(player.GetComponent<GenericAoeAttack>().AimAoeAttack);
                UiCamera
                    .GetComponent<CustomInputManager>()
                    .AssignInputToAimPosition("y", "joystick", aimEvent);

                UnityEvent<Vector2> attackEvent = new UnityEvent<Vector2>();
                attackEvent.AddListener(player.GetComponent<GenericAoeAttack>().ExecuteAoeAttack);
                UiCamera
                    .GetComponent<CustomInputManager>()
                    .AssignInputToAbilityExecution("y", "joystick", attackEvent);
            }
        }
    }
}
