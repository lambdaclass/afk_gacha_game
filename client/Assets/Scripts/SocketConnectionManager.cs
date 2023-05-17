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
    public LevelManager levelManager;
    public CinemachineCameraController camera;
    public Character prefab;
    public List<GameObject> players;
    public static List<GameObject> playersStatic;
    public Queue<PositionUpdate> positionUpdates = new Queue<PositionUpdate>();

    [Tooltip("Session ID to connect to. If empty, a new session will be created")]
    public string session_id = "";
    public static string sessionStatic_id = "";

    [Tooltip("IP to connect to. If empty, localhost will be used")]
    public string server_ip = "localhost";

    WebSocket ws;

    public static SocketConnectionManager Instance;
    private int playerCount = 0;
    private int playerId;

    public GameStateUpdate gameUpdate;

    public class GameResponse
    {
        public List<Player> players { get; set; }
    }

    public class Session
    {
        public string session_id { get; set; }
    }

    public struct PositionUpdate
    {
        public long x;
        public long y;
        public int player_id;
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

    public void Awake()
    {
        this.session_id = LobbyConnection.Instance.GameSession;
        Instance = this;
        playersStatic = this.players;
        sessionStatic_id = this.session_id;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Send the player's action every 30 ms approximately.
        playerId = LobbyConnection.Instance.playerId;

        if (this.session_id.IsNullOrEmpty())
        {
            StartCoroutine(GetRequest("http://" + server_ip + ":4000/new_session"));
        }
        else
        {
            ConnectToSession(this.session_id);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // se mueve
        // while (positionUpdates.TryDequeue(out var positionUpdate))
        // {
        //     this.players[positionUpdate.player_id].transform.position = new Vector3(
        //         positionUpdate.x / 10f - 50.0f,
        //         this.players[positionUpdate.player_id].transform.position.y,
        //         positionUpdate.y / 10f + 50.0f
        //     );
        // }
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
                    Session session = JsonConvert.DeserializeObject<Session>(
                        webRequest.downloadHandler.text
                    );
                    Debug.Log("Creating and joining Session ID: " + session.session_id);
                    ConnectToSession(session.session_id);
                    break;
            }
        }
    }

    private void ConnectToSession(string session_id)
    {
        print("ws://" + server_ip + ":4000/play/" + session_id + "/" + playerId);
        ws = new WebSocket("ws://" + server_ip + ":4000/play/" + session_id + "/" + playerId);
        ws.OnMessage += OnWebSocketMessage;
        ws.OnError += (sender, e) =>
        {
            Debug.Log(
                "Error received from: " + ((WebSocket)sender).Url + ", Data: " + e.Exception.Message
            );
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
            // Se mueve
            GameStateUpdate game_update = Serializer.Deserialize<GameStateUpdate>(
                (ReadOnlySpan<byte>)e.RawData
            );

            this.gameUpdate = game_update;
        }
    }

    public void SendAction(ClientAction action)
    {
        using (var stream = new MemoryStream())
        {
            Serializer.Serialize(stream, action);
            var msg = stream.ToArray();
            ws.Send(msg);
        }
    }
}
