using System.Collections;
using System.Collections.Generic;
using WebSocketSharp;
using Newtonsoft.Json;
using UnityEngine;
using static SocketConnectionManager;
using UnityEngine.Networking;
using ProtoBuf;
using System;

public class LobbyConnection : MonoBehaviour
{

    [Tooltip("Matchmaking session ID to connect to. If empty, a new lobby will be created")]
    public string matchmaking_id = "";

    [Tooltip("IP to connect to. If empty, localhost will be used")]
    public string server_ip = "localhost";

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
    void Start()
    {
        StartCoroutine(GetLobbies("http://" + server_ip + ":4000/current_lobbies"));

        if (this.matchmaking_id.IsNullOrEmpty())
        {
            StartCoroutine(GetRequest("http://" + server_ip + ":4000/new_lobby"));
        }
        else
        {
            ConnectToSession(this.matchmaking_id);
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
                    Debug.Log("Creating and joining lobby ID: " + session.lobby_id);
                    ConnectToSession(session.lobby_id);
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
                    response.lobbies.ForEach((lobby) =>
                    {
                        Debug.Log("A lobby id: " + lobby);
                    });
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
        Debug.Log("The message is: " + e.Data);
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ws.Send("un mensajito");
        }   
    }
}
