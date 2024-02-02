using System;
using System.Collections;
using NativeWebSocket;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.IO;
using UnityEngine;
using Protobuf;
using System.Collections.Generic;

public class SocketConnection : MonoBehaviour{
    WebSocket ws;

    public static SocketConnection Instance;

    public bool connected = false;


    void Awake()
    {
        Init();
        Instance.GetUser();
    }

    public void Init()
    {
        if (Instance != null)
        {
            if (this.ws != null)
            {
                this.ws.Close();
            }
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (ws != null)
        {
            ws.DispatchMessageQueue();
        }
#endif
        if(!connected)
        {
            connected = true;
            ConnectToSession();
        }
    }
    
    private void ConnectToSession()
    {
        // string url = $"ws://localhost:4000/2/{Guid.NewGuid().GetHashCode()}";
        string url = $"ws://localhost:4001/2";
        Debug.Log($"url: {url}");
        ws = new WebSocket(url);
        ws.OnMessage += OnWebSocketMessage;
        ws.OnClose += OnWebsocketClose;
        ws.OnOpen += () =>
        {
            Debug.Log("opened connection");
        };
        ws.OnError += (e) =>
        {
            Debug.LogError("Received error: " + e);
        };
        ws.Connect();
    }

    private void OnWebSocketMessage(byte[] data)
    {
        try
        {
            WebSocketResponse webSocketResponse = WebSocketResponse.Parser.ParseFrom(data);

            print($"request type case {webSocketResponse.ResponseTypeCase}");

            switch (webSocketResponse.ResponseTypeCase)
            {
                case WebSocketResponse.ResponseTypeOneofCase.User:
                    List<Character> availableCharacters = GlobalUserData.Instance.AvailableCharacters;
                    List<Unit> units = new List<Unit>();
                    var enumerator = webSocketResponse.User.Units.GetEnumerator();
                    while(enumerator.MoveNext()) {
                        Unit unit = new Unit{
                            id = enumerator.Current.Id,
                            tier = (int)enumerator.Current.Tier,
                            character = availableCharacters.Find(character => character.name.ToLower() == enumerator.Current.Character.Name.ToLower()),
                            // We currently don't get the rank from the backend so it's hardcoded
                            rank = Rank.Star1,
                            level = (int)enumerator.Current.UnitLevel,
                            slot = (int?)enumerator.Current.Slot,
                            selected = enumerator.Current.Selected
                        };
                        units.Add(unit);
                    }

                    GlobalUserData.Instance.User = new User{
                        id = webSocketResponse.User.Id,
                        username = webSocketResponse.User.Username,
                        units = units
                    };
                    break;
                default:
                    Debug.Log("Request case not handled");
                    break;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("InvalidProtocolBufferException: " + e);
        }
    }

    private void OnWebsocketClose(WebSocketCloseCode closeCode)
    {
        if (closeCode != WebSocketCloseCode.Normal)
        {
            // Errors.Instance.HandleNetworkError(connectionTitle, connectionDescription);
            Debug.LogError("Your connection to the server has been lost.");
        }
        else
        {
            Debug.Log("ServerConnection websocket closed normally.");
        }
    }

    private void SendGameAction<T>(IMessage<T> action)
        where T : IMessage<T>
    {
        using (var stream = new MemoryStream())
        {
            action.WriteTo(stream);
            var msg = stream.ToArray();

            if(ws != null) {
                ws.Send(msg);
            }
        }
    }

    public void GetUser()
    {
        Debug.Log("get user");
        GetUser getUserAction = new GetUser{
            UserId = "2123cce2-4a71-4b8d-a95e-d519e5935cc9"
        };
        WebSocketRequest request = new WebSocketRequest{
            GetUser = getUserAction
        };
        SendGameAction(request);
    }

    // public static void SelectUnit(int unitId, int slotId, int userId)
    // {
    //     SelectUnitAction selectUnitAction = new SelectUnitAction(unitId, slotId, userId);
    //     SendGameAction(selectUnitAction);
    // }

    // public static void DeselectUnit(int unitId, int userId)
    // {
    //     DeselectUnitAction deselectUnitAction = new DeselectUnitAction(unitId, userId);
    //     SendGameAction(deselectUnitAction);
    // }
}
