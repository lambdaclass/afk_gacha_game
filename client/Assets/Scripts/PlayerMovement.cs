using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    void Start()
    {
        // Send the player's action every 30 ms approximately.
        float tickRate = 1f / 30f;
        InvokeRepeating("SendAction", tickRate, tickRate);
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

}
