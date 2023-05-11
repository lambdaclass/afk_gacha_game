using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ProtoBuf;
using UnityEngine;
using UnityEngine.Networking;
using WebSocketSharp;
using static SocketConnectionManager;

public class LobbyConnection : MonoBehaviour
{

    [Tooltip("Matchmaking session ID to connect to. If empty, a new lobby will be created")]
    public string matchmaking_id = "";

    [Tooltip("IP to connect to. If empty, localhost will be used")]
    public string server_ip = "localhost";
    
    [SerializeField] GameObject lobbyItemPrefab;

    [SerializeField] Transform lobbyListContainer;
    public List<GameObject> lobbiesList;

    WebSocket ws;

    public class Session
    {
        public string lobby_id { get; set; }
    }

    public class LobbiesResponse
    {
        public List<string> lobbies { get; set; }
    }

    // Start is called before the first frame update

    public void CreateLobby()
    {
        if (this.matchmaking_id.IsNullOrEmpty())
        {
            StartCoroutine(GetRequest("http://" + server_ip + ":4000/new_lobby"));
        }
        else
        {
            ConnectToSession(this.matchmaking_id);
        }
    }

    public void Init()
    {
        CreateLobby();
    }

    void Start()
    {
        StartCoroutine(GetLobbies("http://" + server_ip + ":4000/current_lobbies"));
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
                    Debug.Log("Creating and joining lobby ID: " + session.lobby_id);
                    break;
            }
        }
    }

    IEnumerator GetLobbies(string uri)
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
                    LobbiesResponse response = JsonConvert.DeserializeObject<LobbiesResponse>(webRequest.downloadHandler.text);
                    Debug.Log("Lobbies response: " + response.lobbies);
                    lobbiesList = response.lobbies.Select(l => {
                        Debug.Log("A lobby id: " + l);
                        GameObject lobbyItem = Instantiate(lobbyItemPrefab, lobbyListContainer);
                        lobbyItem.GetComponent<LobbyItem>().setId(l); // TODO: check this
                        return lobbyItem;
                    }).ToList();

                    break;
            }
        }
    }

    private void ConnectToSession(string session_id)
    {
        ws = new WebSocket("ws://" + server_ip + ":4000/matchmaking/" + session_id);
        ws.OnMessage += OnWebSocketMessage;
        ws.OnError += (sender, e) =>
        {
            Debug.Log("Error received from: " + ((WebSocket)sender).Url + ", Data: " + e.Exception.Message);
        };
        ws.Connect();
    }

    private void OnWebSocketMessage(object sender, MessageEventArgs e)
    {
        if (e.Data.Contains("GAME_ID"))
        {
            string game_id = e.Data.Split(": ")[1];
            print("The game id is: " + game_id);
        }
        else
        {
            Debug.Log("Message received is: " + e.Data);
        }
    }

    public void StartLobby()
    {
        ws.Send("START_GAME");
    }

    // private void Update()
    // {
    //     if (Input.GetKey(KeyCode.Space))
    //     {
    //         ws.Send("START_GAME");
    //     }
    // }

}
