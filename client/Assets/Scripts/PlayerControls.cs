using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public static void SendAction()
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
}
