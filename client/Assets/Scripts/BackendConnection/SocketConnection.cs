using System;
using System.Collections;
using NativeWebSocket;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.IO;
using UnityEngine;

public class SocketConnection : MonoBehaviour{
    WebSocket ws;

    public static SocketConnection Instance;

    public bool connected = false;


    void Start()
    {
        Init();
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
        // StartCoroutine(IsGameCreated());
        if(!connected)
        {
            connected = true;
            ConnectToSession();
        }
    }

    // private IEnumerator IsGameCreated()
    // {
    //     // yield return new WaitUntil(() => !String.IsNullOrEmpty(SessionParameters.GameId));

    //     // this.sessionId = ServerConnection.Instance.GameSession;
    //     // this.serverIp = ServerConnection.Instance.serverIp;
    //     // this.serverTickRate_ms = ServerConnection.Instance.serverTickRate_ms;
    //     // this.serverHash = ServerConnection.Instance.serverHash;
    //     // this.clientId = ServerConnection.Instance.clientId;
    //     // this.reconnect = ServerConnection.Instance.reconnect;

    //     if (!connected)
    //     {
    //         connected = true;
    //         ConnectToSession();
    //     }
    // }
    
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
        Debug.Log("Get message");
        try
        {
            WebSocketRequest webSocketRequest = WebSocketRequest.Parser.ParseFrom(data);

            print($"request type case {webSocketRequest.RequestTypeCase}");

            switch (webSocketRequest.RequestTypeCase)
            {
                case WebSocketRequest.RequestTypeOneofCase.GetUser:
                    Debug.Log($"userId: {webSocketRequest.GetUser.UserId}");
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
            ws.Send(msg);
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
