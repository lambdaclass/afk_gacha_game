using System;
using System.Collections;
using NativeWebSocket;
using Google.Protobuf;
using Google.Protobuf.Collections;
using System.IO;
using UnityEngine;
using Protobuf;
using System.Collections.Generic;
using System.Threading.Tasks;

public class SocketConnection : MonoBehaviour {
    WebSocket ws;

    public static SocketConnection Instance;

    public bool connected = false;

    private WebSocketMessageEventHandler currentMessageHandler;

    int counter = 0;

    async void Awake()
    {
        await Init();
        StartCoroutine(GetUserAndContinue());
    }

    public async Task Init()
    {
        if (Instance != null)
        {
            if (this.ws != null)
            {
                await this.ws.Close();
            }
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if(!connected)
        {
            connected = true;
            ConnectToSession();
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
        // if(!connected)
        // {
        //     connected = true;
        //     ConnectToSession();
        // }
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

            // print($"WebSocketResponse type: {webSocketResponse.ResponseTypeCase}");

            List<Character> availableCharacters = GlobalUserData.Instance.AvailableCharacters;

            switch (webSocketResponse.ResponseTypeCase)
            {
                case WebSocketResponse.ResponseTypeOneofCase.User:
                    HandleUserResponse(webSocketResponse.User, availableCharacters);
                    break;
                case WebSocketResponse.ResponseTypeOneofCase.Error:
                    Debug.LogError("response type error");
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

    private List<CampaignData> ParseCampaignsFromResponse(Protobuf.Campaigns campaignsData, List<Character> availableCharacters)
    {
        List<CampaignData> campaigns = new List<CampaignData>();

        for(int campaignIndex = 0; campaignIndex < campaignsData.Campaigns_.Count; campaignIndex++)
        {
            List<LevelData> levels = new List<LevelData>();

            for(int levelIndex = 0; levelIndex < campaignsData.Campaigns_[campaignIndex].Levels.Count; levelIndex++)
            {
                Protobuf.Level level = campaignsData.Campaigns_[campaignIndex].Levels[levelIndex];
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
                status = campaignIndex == 0 ? CampaignData.Status.Unlocked : CampaignData.Status.Locked,
                levels = levels
            });
        }

        return campaigns;
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

    // Better name for this method?
    private IEnumerator GetUserAndContinue()
    {
        if(GlobalUserData.Instance.User == null) {
            yield return new WaitUntil(() => ws.WebSocketConnectionState == System.Net.WebSockets.WebSocketState.Open);
            string userId = PlayerPrefs.GetString("userId");
            if(String.IsNullOrEmpty(userId)) {
                CreateUser("testUser", (user) => {
                    PlayerPrefs.SetString("userId", user.id);
                    GlobalUserData.Instance.User = user;
                });
            }
            else {
                GetUser(userId, (user) => {
                    PlayerPrefs.SetString("userId", user.id);
                    GlobalUserData.Instance.User = user;
                });
            }
        }
    }

    public void GetUser(string userId, Action<User> onGetUserDataReceived)
    {
        GetUser getUserRequest = new GetUser{
            UserId = userId
        };
        WebSocketRequest request = new WebSocketRequest{
            GetUser = getUserRequest
        };
        currentMessageHandler = (data) => AwaitGetUserResponse(data, onGetUserDataReceived);
        ws.OnMessage += currentMessageHandler;
        ws.OnMessage -= OnWebSocketMessage;
        SendWebSocketMessage(request);
    }

    public void GetUserByUsername(string username, Action<User> onGetUserDataReceived)
    {
        print("GetUserByUsername");
        counter++;
        if(counter < 5)
        {
            GetUserByUsername getUserByUsernameRequest = new GetUserByUsername{
                Username = username
            };
            WebSocketRequest request = new WebSocketRequest{
                GetUserByUsername = getUserByUsernameRequest
            };
            currentMessageHandler = (data) => AwaitGetUserResponse(data, onGetUserDataReceived);
            ws.OnMessage += currentMessageHandler;
            ws.OnMessage -= OnWebSocketMessage;
            SendWebSocketMessage(request);
        }
    }

    private void AwaitGetUserResponse(byte[] data, Action<User> onGetUserDataReceived)
    {
        try
        {
            WebSocketResponse webSocketResponse = WebSocketResponse.Parser.ParseFrom(data);
            print($"Get User response type: {webSocketResponse.ResponseTypeCase}");
            if(webSocketResponse.ResponseTypeCase == WebSocketResponse.ResponseTypeOneofCase.User) {
                ws.OnMessage -= currentMessageHandler;

                List<Unit> units = new List<Unit>();

                foreach (var userUnit in webSocketResponse.User.Units)
                {
                    Unit unit = CreateUnitFromData(userUnit, GlobalUserData.Instance.AvailableCharacters);
                    units.Add(unit);
                }

                User user = new User
                {
                    id = webSocketResponse.User.Id,
                    username = webSocketResponse.User.Username,
                    units = units
                };

                onGetUserDataReceived?.Invoke(user);
                ws.OnMessage += OnWebSocketMessage;
            }
            else if(webSocketResponse.ResponseTypeCase == WebSocketResponse.ResponseTypeOneofCase.Error) {
                Debug.LogError($"error reason: {webSocketResponse.Error.Reason}");
                switch(webSocketResponse.Error.Reason) {
                    case "not_found":
                        CreateUser("testUser", onGetUserDataReceived);
                        break;
                    case "username_taken":
                        Debug.LogError("Tried to create user, username was taken");
                        break;
                    default:
                        Debug.LogError(webSocketResponse.Error.Reason);
                        break;
                }
            }
        } catch (Exception e) {
            Debug.LogError("InvalidProtocolBufferException: " + e);
        }
    }

    public void CreateUser(string username, Action<User> onGetUserDataReceived)
    {
        print("create user");
        CreateUser createUserRequest = new CreateUser{
            Username = username
        };
        WebSocketRequest request = new WebSocketRequest{
            CreateUser = createUserRequest
        };
        currentMessageHandler = (data) => AwaitGetUserResponse(data, onGetUserDataReceived);
        ws.OnMessage += currentMessageHandler;
        ws.OnMessage -= OnWebSocketMessage;
        SendWebSocketMessage(request);
    }

    public void GetCampaigns(string userId, Action<List<CampaignData>> onCampaignDataReceived)
    {
        GetCampaigns getCampaignsRequest = new GetCampaigns{
            UserId = userId
        };
        WebSocketRequest request = new WebSocketRequest{
            GetCampaigns = getCampaignsRequest
        };
        SendWebSocketMessage(request);
        currentMessageHandler = (data) => AwaitGetCampaignsResponse(data, onCampaignDataReceived);
        ws.OnMessage += currentMessageHandler;
        ws.OnMessage -= OnWebSocketMessage;
    }

    private void AwaitGetCampaignsResponse(byte[] data, Action<List<CampaignData>> onCampaignDataReceived)
    {
        try
        {
            WebSocketResponse webSocketResponse = WebSocketResponse.Parser.ParseFrom(data);
            if(webSocketResponse.ResponseTypeCase == WebSocketResponse.ResponseTypeOneofCase.Campaigns) {
                ws.OnMessage -= currentMessageHandler;
                List<Character> availableCharacters = GlobalUserData.Instance.AvailableCharacters;
                List<CampaignData> campaigns = ParseCampaignsFromResponse(webSocketResponse.Campaigns, availableCharacters);
                onCampaignDataReceived?.Invoke(campaigns);
                ws.OnMessage += OnWebSocketMessage;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("InvalidProtocolBufferException: " + e);
        }
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

    public void Battle(string userId, string levelId, Action<bool> onBattleResultReceived) {
        FightLevel fightLevelRequest = new FightLevel {
            UserId = userId,
            LevelId = levelId
        };
        WebSocketRequest request = new WebSocketRequest {
            FightLevel = fightLevelRequest
        };
        SendWebSocketMessage(request);
        ws.OnMessage += (data) => AwaitBattleResponse(data, onBattleResultReceived);
    }

    private void AwaitBattleResponse(byte[] data, Action<bool> onBattleResultReceived)
    {
        try
        {
            WebSocketResponse webSocketResponse = WebSocketResponse.Parser.ParseFrom(data);
            if(webSocketResponse.ResponseTypeCase == WebSocketResponse.ResponseTypeOneofCase.BattleResult) {
                bool battleResult = webSocketResponse.BattleResult.Result == "win";
                onBattleResultReceived?.Invoke(battleResult);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("InvalidProtocolBufferException: " + e);
        }
    }

    private async void OnApplicationQuit()
    {
        await this.ws.Close();
    }
}
