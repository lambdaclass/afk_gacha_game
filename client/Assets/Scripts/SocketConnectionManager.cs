using WebSocketSharp;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Net;
using System;
using System.Xml.Linq;

public class SocketConnectionManager : MonoBehaviour
{
    public List<GameObject> players;
    public Queue<PositionUpdate> positionUpdates = new Queue<PositionUpdate>();

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

    public struct PositionUpdate
    {
        public int x;
        public int y;
        public int player_id;
    }

    public class Position
    {
        public int x { get; set; }
        public int y { get; set; }
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
            Dictionary<string, string> move = new Dictionary<string, string>();
            move.Add("action", "move");
            move.Add("value", "up");
            string msg = JsonConvert.SerializeObject(move);
            ws.Send(msg);
        }
        if (Input.GetKey(KeyCode.A))
        {
            Dictionary<string, string> move = new Dictionary<string, string>();
            move.Add("action", "move");
            move.Add("value", "left");
            string msg = JsonConvert.SerializeObject(move);
            ws.Send(msg);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Dictionary<string, string> move = new Dictionary<string, string>();
            move.Add("action", "move");
            move.Add("value", "right");
            string msg = JsonConvert.SerializeObject(move);
            ws.Send(msg);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Dictionary<string, string> move = new Dictionary<string, string>();
            move.Add("action", "move");
            move.Add("value", "down");
            string msg = JsonConvert.SerializeObject(move);
            ws.Send(msg);
        }
    }

    // Update is called once per frame
    void Update()
    {
        while (positionUpdates.TryDequeue(out var positionUpdate))
        {
            this.players[positionUpdate.player_id].transform.position = new Vector3(positionUpdate.x - 50.0f, 0.0f, positionUpdate.y + 50.0f);
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
        //Debug.Log("Message received from: " + ((WebSocket)sender).Url + ", Data: " + e.Data);

        if (e.Data == "OK" || e.Data.Contains("CONNECTED_TO"))
        {
            //Debug.Log("Nothing to do");
        }
        else
        {
            GameResponse game_response = JsonConvert.DeserializeObject<GameResponse>(e.Data);
            Debug.Log(game_response);

            for (int i = 0; i < game_response.players.Count; i++)
            {
                var player = this.players[i];

                var new_position = game_response.players[i].position;
                positionUpdates.Enqueue(new PositionUpdate { x = new_position.y, y = -new_position.x, player_id = i });
                //Debug.Log("i: " + i.ToString());
            }
        }
    }
}
