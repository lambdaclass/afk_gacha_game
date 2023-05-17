using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Queue<PositionUpdate> positionUpdates = new Queue<PositionUpdate>();
    public struct PositionUpdate
    {
        public long x;
        public long y;
        public int player_id;
    }
    void Start()
    {
        // Send the player's action every 30 ms approximately.
        float tickRate = 1f / 30f;
        InvokeRepeating("SendAction", tickRate, tickRate);
        UpdatePlayerPositions();
    }

    void Update()
    {
        while (positionUpdates.TryDequeue(out var positionUpdate))
        {
            SocketConnectionManager.Instance.players[positionUpdate.player_id].transform.position = new Vector3(
                positionUpdate.x / 10f - 50.0f,
                SocketConnectionManager.Instance.players[positionUpdate.player_id].transform.position.y,
                positionUpdate.y / 10f + 50.0f
            );
        }
    }

    void SendAction()
    {
        // Se mueve
        // if (ws == null)
        // {
        //     return;
        // }
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
    }

    void UpdatePlayerPositions()
    {
        GameStateUpdate game_update = SocketConnectionManager.Instance.gameUpdate;
        for (int i = 0; i < game_update.Players.Count; i++)
        {
            var player = SocketConnectionManager.Instance.players[i];
            var new_position = game_update.Players[i].Position;
            positionUpdates.Enqueue(new PositionUpdate { x = (long)new_position.Y, y = -((long)new_position.X), player_id = i });
        }
    }
}
