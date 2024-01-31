using System;
using System.Collections;
using NativeWebSocket;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.IO;

public static class SocketConnection {
    static WebSocket ws;
    
    private static void ConnectToSession()
    {
        string url = "localhost:4000/testSocketUrl";
        ws = new WebSocket(url);
        ws.OnMessage += OnWebSocketMessage;
        ws.OnClose += OnWebsocketClose;
        // ws.OnOpen += () =>
        // {
        //     LobbySession = "sessionId";
        // };
        ws.Connect();
    }

    private static void OnWebSocketMessage(byte[] data)
    {
        try
        {
            GameEvent gameEvent = GameEvent.Parser.ParseFrom(data);

            switch (gameEvent.EventCase)
            {
                case GameEvent.EventOneofCase.GetUser:
                    
                    break;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("InvalidProtocolBufferException: " + e);
        }
    }

    private static void OnWebsocketClose(WebSocketCloseCode closeCode)
    {
        if (closeCode != WebSocketCloseCode.Normal)
        {
            // Errors.Instance.HandleNetworkError(connectionTitle, connectionDescription);
            Console.WriteLine("Your connection to the server has been lost.");
        }
        else
        {
            Console.WriteLine("ServerConnection websocket closed normally.");
        }
    }

    private static void SendGameAction<T>(IMessage<T> action)
        where T : IMessage<T>
    {
        using (var stream = new MemoryStream())
        {
            action.WriteTo(stream);
            var msg = stream.ToArray();
            ws.Send(msg);
        }
    }

    public static void GetUser()
    {
        int userId = 2;
        GetUserAction getUserAction = new GetUserAction(userId);
        SendGameAction(getUserAction);
    }

    public static void SelectUnit(int unitId, int slotId, int userId)
    {
        SelectUnitAction selectUnitAction = new SelectUnitAction(unitId, slotId, userId);
        SendGameAction(selectUnitAction);
    }

    public static void DeselectUnit(int unitId, int userId)
    {
        DeselectUnitAction deselectUnitAction = new DeselectUnitAction(unitId, userId);
        SendGameAction(deselectUnitAction);
    }
}
