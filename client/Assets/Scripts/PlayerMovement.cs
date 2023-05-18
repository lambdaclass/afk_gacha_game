using System.Collections.Generic;
using UnityEngine;
using MoreMountains.TopDownEngine;

public class PlayerMovement : MonoBehaviour
{
    public Queue<PlayerUpdate> playerUpdates = new Queue<PlayerUpdate>();

    public struct PlayerUpdate
    {
        public long x;
        public long y;
        public int player_id;
        public long health;
        public PlayerAction action;
    }

    public enum PlayerAction
    {
        Nothing = 0,
        Attacking = 1,
    }

    void Start()
    {
        // Send the player's action every 30 ms approximately.
        float tickRate = 1f / 30f;
        InvokeRepeating("SendAction", tickRate, tickRate);
    }

    void Update()
    {
        if (
            SocketConnectionManager.Instance.gameUpdate != null
            && SocketConnectionManager.Instance.players.Count > 0
            && SocketConnectionManager.Instance.gameUpdate.Players.Count > 0
        )
        {
            UpdatePlayerPositions();
            MakePlayerMove();
        }
    }

    void SendAction()
    {
        if (Input.GetKey(KeyCode.W))
        {
            ClientAction action = new ClientAction
            {
                Action = Action.Move,
                Direction = Direction.Up
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKey(KeyCode.A))
        {
            ClientAction action = new ClientAction
            {
                Action = Action.Move,
                Direction = Direction.Left
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKey(KeyCode.D))
        {
            ClientAction action = new ClientAction
            {
                Action = Action.Move,
                Direction = Direction.Right
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKey(KeyCode.S))
        {
            ClientAction action = new ClientAction
            {
                Action = Action.Move,
                Direction = Direction.Down
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKey(KeyCode.E))
        {
            ClientAction action = new ClientAction { Action = Action.AttackAoe };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            // This sends the action
            ClientAction action = new ClientAction
            {
                Action = Action.Attack,
                Direction = Direction.Down
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            ClientAction action = new ClientAction
            {
                Action = Action.Attack,
                Direction = Direction.Up
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ClientAction action = new ClientAction
            {
                Action = Action.Attack,
                Direction = Direction.Right
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            ClientAction action = new ClientAction
            {
                Action = Action.Attack,
                Direction = Direction.Left
            };
            SocketConnectionManager.Instance.SendAction(action);
        }
    }

    void MakePlayerMove()
    {
        while (playerUpdates.TryDequeue(out var playerUpdate))
        {
            SocketConnectionManager.Instance.players[playerUpdate.player_id].transform.position =
                new Vector3(
                    playerUpdate.x / 10f - 50.0f,
                    SocketConnectionManager.Instance.players[playerUpdate.player_id]
                        .transform
                        .position
                        .y,
                    playerUpdate.y / 10f + 50.0f
                );
            Health healthComponent = SocketConnectionManager.Instance.players[
                playerUpdate.player_id
            ].GetComponent<Health>();
            healthComponent.SetHealth(playerUpdate.health);

            bool isAttacking = playerUpdate.action == PlayerAction.Attacking;
            SocketConnectionManager.Instance.players[playerUpdate.player_id]
                .GetComponent<AttackController>()
                .SwordAttack(isAttacking);
            if (isAttacking)
            {
                print("attack");
            }
        }
    }

    void UpdatePlayerPositions()
    {
        GameStateUpdate game_update = SocketConnectionManager.Instance.gameUpdate;
        for (int i = 0; i < SocketConnectionManager.Instance.players.Count; i++)
        {
            var new_position = game_update.Players[i].Position;
            playerUpdates.Enqueue(
                new PlayerUpdate
                {
                    x = (long)new_position.Y,
                    y = -((long)new_position.X),
                    player_id = i,
                    health = game_update.Players[i].Health,
                    action = (PlayerAction)game_update.Players[i].Action,
                }
            );
        }
    }
}
