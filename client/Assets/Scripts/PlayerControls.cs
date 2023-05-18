using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public static void SendAction()
    {
        if (Input.GetKey(KeyCode.W))
        {
            SendAction(Action.Move, Direction.Up);
        }
        if (Input.GetKey(KeyCode.A))
        {
            SendAction(Action.Move, Direction.Left);
        }
        if (Input.GetKey(KeyCode.D))
        {
            SendAction(Action.Move, Direction.Right);
        }
        if (Input.GetKey(KeyCode.S))
        {
            SendAction(Action.Move, Direction.Down);
        }
        if (Input.GetKey(KeyCode.E))
        {
            ClientAction action = new ClientAction { Action = Action.AttackAoe };
            SocketConnectionManager.Instance.SendAction(action);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            SendAction(Action.Attack, Direction.Down);
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            SendAction(Action.Attack, Direction.Up);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SendAction(Action.Attack, Direction.Right);
        }
        if (Input.GetKeyDown(KeyCode.H))
        {
            SendAction(Action.Attack, Direction.Left);
        }
    }

    private static void SendAction(Action action, Direction direction)
    {
        ClientAction clientAction = new ClientAction { Action = action, Direction = direction };
        SocketConnectionManager.Instance.SendAction(clientAction);
    }
}
