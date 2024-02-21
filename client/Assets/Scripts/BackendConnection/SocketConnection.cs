using System;
using Google.Protobuf;
using System.IO;
using UnityEngine;
using Protobuf;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;

public static class SocketConnection
{
    static ClientWebSocket webSocketConnection;
    static String URL = "ws://localhost:4001/2";

    public static async Task Connect() {
        // handshake
        webSocketConnection = new ClientWebSocket();
        Uri wsUri = new Uri(URL);
        try {
            webSocketConnection.Options.SetRequestHeader("Origin", "http://example.com");
            await webSocketConnection.ConnectAsync(wsUri, CancellationToken.None);
            _ = ReceiveMessages();
        } catch(Exception ex) {
            Debug.Log(ex.Message);
        }
    }

    static async Task ReceiveMessages()
    {
        List<byte> messageBytes = new List<byte>();
        byte[] receiveBuffer = new byte[1024];
        while (webSocketConnection.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result = null;
            do
            {
                result = await webSocketConnection.ReceiveAsync(new ArraySegment<byte>(receiveBuffer), CancellationToken.None);
                for (int i = 0; i < result.Count; i++)
                {
                    messageBytes.Add(receiveBuffer[i]);
                }
            }
            while (!result.EndOfMessage);
            
            if (result.MessageType == WebSocketMessageType.Binary)
            {
                var webSocketResponse = WebSocketResponse.Parser.ParseFrom(messageBytes.ToArray());
                Debug.Log("received user: " + webSocketResponse.User.Username);
                
                messageBytes.Clear();
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                await webSocketConnection.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            }
            else
            {
                Debug.LogError("Message from server is not binary");
            }
        }
    }

    public static void GetUser(string userId)
    {
        try {
            GetUser getUserRequest = new GetUser{
                UserId = userId
            };
            WebSocketRequest request = new WebSocketRequest{
                GetUser = getUserRequest
            };
            SendWebSocketMessage(request);
        } catch (Exception e) {
            Debug.LogError(e.Message);
        }
    }

    private static void SendWebSocketMessage<T>(IMessage<T> message)
    where T : IMessage<T>
    {
        try {
            using (var stream = new MemoryStream())
            {
                message.WriteTo(stream);
                var msg = stream.ToArray();
                webSocketConnection.SendAsync(msg, WebSocketMessageType.Binary, true, CancellationToken.None);
            }
        } catch(Exception e) {
            Debug.LogError(e.Message);
        }

    }
}
