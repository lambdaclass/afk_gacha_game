using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.Networking;
using System.Net;
using System;
using System.Xml.Linq;
using ProtoBuf;
using MoreMountains.TopDownEngine;

public class SocketConnectionManager : MonoBehaviour
{
    public List<GameObject> players;
    public Queue<PlayerUpdate> playerUpdates = new Queue<PlayerUpdate>();

    [Tooltip("Session ID to connect to. If empty, a new session will be created")]
    public string session_id = "";

    [Tooltip("IP to connect to. If empty, localhost will be used")]
    public string server_ip = "localhost";

    WebSocket ws;

    public class GameResponse
    {
        public List<Player> players { get; set; }
    }

    public class Session
    {
        public string session_id { get; set; }
    }

    public struct PlayerUpdate
    {
        public long x;
        public long y;
        public int player_id;
        public long health;
    }

    public class Position
    {
        public long x { get; set; }
        public long y { get; set; }
    }

    public class Player
    {
        public int id { get; set; }
        public int health { get; set; }
        public Position position { get; set; }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Send the player's action every 30 ms approximately.
        InvokeRepeating("sendAction", 0.03333333f, 0.03333333f);

        if (this.session_id.IsNullOrEmpty())
        {
            StartCoroutine(GetRequest("http://" + server_ip + ":4000/new_session"));
        }
        else
        {
            ConnectToSession(this.session_id);
        }
    }

    void sendAction()
    {
        if (ws == null)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            ClientAction action = new ClientAction { Action = Action.Move, Direction = Direction.Up};
            SendAction(action);
        }
        if (Input.GetKey(KeyCode.A))
        {
            ClientAction action = new ClientAction { Action = Action.Move, Direction = Direction.Left};
            SendAction(action);
        }
        if (Input.GetKey(KeyCode.D))
        {
            ClientAction action = new ClientAction { Action = Action.Move, Direction = Direction.Right};
            SendAction(action);
        }
        if (Input.GetKey(KeyCode.S))
        {
            ClientAction action = new ClientAction { Action = Action.Move, Direction = Direction.Down};
            SendAction(action);
        }
        if (Input.GetKey(KeyCode.J))
        {
            ClientAction action = new ClientAction { Action = Action.Attack, Direction = Direction.Down};
            SendAction(action);
        }
        if (Input.GetKey(KeyCode.U))
        {
            ClientAction action = new ClientAction { Action = Action.Attack, Direction = Direction.Up};
            SendAction(action);

        }
        if (Input.GetKey(KeyCode.K))
        {
            ClientAction action = new ClientAction { Action = Action.Attack, Direction = Direction.Right};
            SendAction(action);
        }
        if (Input.GetKey(KeyCode.H))
        {
            ClientAction action = new ClientAction { Action = Action.Attack, Direction = Direction.Left};
            SendAction(action);
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (playerUpdates.TryDequeue(out var playerUpdate))
        {
            
            this.players[playerUpdate.player_id].transform.position = new Vector3(playerUpdate.x / 10f - 50.0f, this.players[playerUpdate.player_id].transform.position.y, playerUpdate.y / 10f + 50.0f);
            
            Health healthComponent = this.players[playerUpdate.player_id].GetComponent<Health>();
            healthComponent.SetHealth(playerUpdate.health);
        }
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();
            string[] pages = uri.Split('/');
            int page = pages.Length - 1;
            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    //Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    //Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Session session = JsonConvert.DeserializeObject<Session>(webRequest.downloadHandler.text);
                    Debug.Log("Creating and joining Session ID: " + session.session_id);
                    ConnectToSession(session.session_id);
                    break;

            }
        }
    }

    private void ConnectToSession(string session_id)
    {
        ws = new WebSocket("ws://" + server_ip + ":4000/play/" + session_id);
        ws.OnMessage += OnWebSocketMessage;
        ws.OnError += (sender, e) =>
        {
            Debug.Log("Error received from: " + ((WebSocket)sender).Url + ", Data: " + e.Exception.Message);
        };
        ws.Connect();
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        // Debug.Log("Message received from: " + ((WebSocket)sender).Url + ", Data: " + e.Data);

        if (e.Data == "OK" || e.Data.Contains("CONNECTED_TO"))
        {
            //Debug.Log("Nothing to do");
        }
        else if (e.Data.Contains("ERROR"))
        {
            Debug.Log("Error message: " + e.Data);
        }
        else
        {
            GameStateUpdate game_update = Serializer.Deserialize<GameStateUpdate>((ReadOnlySpan<byte>) e.RawData);

            for (int i = 0; i < game_update.Players.Count; i++)
            {
                var player = this.players[i];
                var new_position = game_update.Players[i].Position;
                playerUpdates.Enqueue(new PlayerUpdate { x = new_position.Y, y = -new_position.X, player_id = i, health = game_update.Players[i].Health});
            }
        }
    }

    private void SendAction(ClientAction action) {
        using (var stream = new MemoryStream()) {
            Serializer.Serialize(stream, action);
            var msg = stream.ToArray();
            ws.Send(msg);
        }
    }
}
