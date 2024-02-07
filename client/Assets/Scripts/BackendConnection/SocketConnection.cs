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
        ws = new WebSocket(url);
        ws.OnMessage += OnWebSocketMessage;
        ws.OnClose += OnWebsocketClose;
        ws.OnOpen += () =>
        {
            // Debug.Log("opened connection");
        };
        ws.OnError += (e) =>
        {
            Debug.LogError("Received error: " + e);
        };
        ws.Connect();
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

    private void SendWebSocketMessage<T>(IMessage<T> message)
        where T : IMessage<T>
    {
        using (var stream = new MemoryStream())
        {
            message.WriteTo(stream);
            var msg = stream.ToArray();

            if(ws != null) {
                ws.Send(msg);
            }
        }
    }
    
    private void OnWebSocketMessage(byte[] data)
    {
        try
        {
            WebSocketResponse webSocketResponse = WebSocketResponse.Parser.ParseFrom(data);
            print($"request type case {webSocketResponse.ResponseTypeCase}");

            List<Character> availableCharacters = GlobalUserData.Instance.AvailableCharacters;

            switch (webSocketResponse.ResponseTypeCase)
            {
                case WebSocketResponse.ResponseTypeOneofCase.User:
                    HandleUserResponse(webSocketResponse.User, availableCharacters);
                    break;
                case WebSocketResponse.ResponseTypeOneofCase.Campaigns:
                    HandleCampaignsResponse(webSocketResponse.Campaigns, availableCharacters);
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

    private void HandleUserResponse(Protobuf.User user, List<Character> availableCharacters)
    {
        List<Unit> units = new List<Unit>();

        foreach (var userUnit in user.Units)
        {
            Unit unit = CreateUnitFromData(userUnit, availableCharacters);
            units.Add(unit);
        }

        GlobalUserData.Instance.User = new User
        {
            id = user.Id,
            username = user.Username,
            units = units
        };
    }

    private void HandleCampaignsResponse(Protobuf.Campaigns campaignsData, List<Character> availableCharacters)
    {
        List<CampaignData> campaigns = new List<CampaignData>();

        foreach (var campaign in campaignsData.Campaigns_)
        {
            List<LevelData> levels = new List<LevelData>();

            for(int levelIndex = 0; levelIndex < campaign.Levels.Count; levelIndex++)
            {
                Protobuf.Level level = campaign.Levels[levelIndex];
                List<Unit> levelUnits = new List<Unit>();

                foreach (var levelUnit in level.Units)
                {
                    Unit unit = CreateUnitFromData(levelUnit, availableCharacters);
                    levelUnits.Add(unit);
                }

                levels.Add(new LevelData
                {
                    id = level.Id,
                    levelNumber = (int)level.LevelNumber,
                    campaign = (int)level.Campaign,
                    units = levelUnits,
                    first = levelIndex == 0
                });
            }

            campaigns.Add(new CampaignData
            {
                levels = levels
            });
        }

        CampaignsMapManager.campaigns = campaigns;
    }

    private Unit CreateUnitFromData(Protobuf.Unit unitData, List<Character> availableCharacters)
    {
        return new Unit
        {
            id = unitData.Id,
            tier = (int)unitData.Tier,
            character = availableCharacters.Find(character => character.name.ToLower() == unitData.Character.Name.ToLower()),
            // We currently don't get the rank from the backend so it's hardcoded
            rank = Rank.Star1,
            level = (int)unitData.UnitLevel,
            slot = (int?)unitData.Slot,
            selected = unitData.Selected
        };
    }

    public void GetUser()
    {
        GetUser getUserRequest = new GetUser{
            UserId = "2123cce2-4a71-4b8d-a95e-d519e5935cc9"
        };
        WebSocketRequest request = new WebSocketRequest{
            GetUser = getUserRequest
        };
        SendWebSocketMessage(request);
    }

    public void GetCampaigns()
    {
        GetCampaigns getCampaignsRequest = new GetCampaigns{
            UserId = "2123cce2-4a71-4b8d-a95e-d519e5935cc9"
        };
        WebSocketRequest request = new WebSocketRequest{
            GetCampaigns = getCampaignsRequest
        };
        SendWebSocketMessage(request);
    }

    public void SelectUnit(string unitId, string userId, int slot)
    {
        SelectUnit selectUnitRequest = new SelectUnit {
            UserId = userId,
            UnitId = unitId,
            Slot = (uint)slot
        };
        WebSocketRequest request = new WebSocketRequest {
            SelectUnit = selectUnitRequest
        };
        SendWebSocketMessage(request);
    }

    public void UnselectUnit(string unitId, string userId)
    {
        UnselectUnit unselectUnitRequest = new UnselectUnit {
            UserId = userId,
            UnitId = unitId
        };
        WebSocketRequest request = new WebSocketRequest {
            UnselectUnit = unselectUnitRequest
        };
        SendWebSocketMessage(request);
    }
}
