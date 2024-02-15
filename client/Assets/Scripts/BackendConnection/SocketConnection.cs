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

    async void Awake()
    {
        await Init();
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
    }

    private async void OnApplicationQuit()
    {
        await this.ws.Close();
    }

    private void ConnectToSession()
    {
        string url = $"ws://localhost:4001/2";
        ws = new WebSocket(url);
        ws.OnMessage += OnWebSocketMessage;
        ws.OnClose += OnWebsocketClose;
        ws.OnOpen += () =>
        {
            GetUserAndContinue();
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

    private Item CreateItemFromData(Protobuf.Item itemData) {
        return new Item
        {
            id = itemData.Id,
            level = (int)itemData.Level,
            userId = itemData.UserId,
            unitId = itemData.UnitId,
            template = new ItemTemplate{
                id = itemData.Template.Id,
                name = itemData.Template.Name,
                type = itemData.Template.Type
            }
        };
    }

    // Better name for this method?
    private void GetUserAndContinue()
    {
        if(GlobalUserData.Instance.User == null) {
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

    private void AwaitGetUserResponse(byte[] data, Action<User> onGetUserDataReceived)
    {
        try
        {
            WebSocketResponse webSocketResponse = WebSocketResponse.Parser.ParseFrom(data);
            if(webSocketResponse.ResponseTypeCase == WebSocketResponse.ResponseTypeOneofCase.User) {
                ws.OnMessage -= currentMessageHandler;

                List<Unit> units = new List<Unit>();

                foreach(var userUnit in webSocketResponse.User.Units)
                {
                    Unit unit = CreateUnitFromData(userUnit, GlobalUserData.Instance.AvailableCharacters);
                    units.Add(unit);
                }

                List<Item> items = new List<Item>();

                foreach(var userItem in webSocketResponse.User.Items)
                {
                    items.Add(CreateItemFromData(userItem));
                }

                Dictionary<Currency, int> currencies = new Dictionary<Currency, int>();

                foreach(var currency in webSocketResponse.User.Currencies) {
                    if (Enum.TryParse<Currency>(currency.Currency.Name, out Currency currencyValue))
                    {
                        currencies.Add(currencyValue, (int)currency.Amount);
                    }
                    else
                    {
                        Debug.LogError($"Currency brought from the backend not found in client: {currency.Currency.Name}");
                    }

                }

                User user = new User
                {
                    id = webSocketResponse.User.Id,
                    username = webSocketResponse.User.Username,
                    units = units,
                    items = items,
                    currencies = currencies
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
            Debug.LogError(e.Message);
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
            Debug.LogError(e.Message);
        }
    }

    public void EquipItem(string userId, string itemId, string unitId, Action<Item> onItemDataReceived) {
        EquipItem equipItemRequest = new EquipItem {
            UserId = userId,
            ItemId = itemId,
            UnitId = unitId
        };
        WebSocketRequest request = new WebSocketRequest {
            EquipItem = equipItemRequest
        };
        SendWebSocketMessage(request);
        currentMessageHandler = (data) => AwaitItemResponse(data, onItemDataReceived);
        ws.OnMessage += currentMessageHandler;
        ws.OnMessage -= OnWebSocketMessage;
    }

    public void UnequipItem(string userId, string itemId, Action<Item> onItemDataReceived) {
        UnequipItem unequipItemRequest = new UnequipItem {
            UserId = userId,
            ItemId = itemId
        };
        WebSocketRequest request = new WebSocketRequest {
            UnequipItem = unequipItemRequest
        };
        SendWebSocketMessage(request);
        currentMessageHandler = (data) => AwaitItemResponse(data, onItemDataReceived);
        ws.OnMessage += currentMessageHandler;
        ws.OnMessage -= OnWebSocketMessage;
    }

    public void LevelUpItem(string userId, string itemId, Action<Item> onItemDataReceived) {
        LevelUpItem levelUpItemRequest = new LevelUpItem {
            UserId = userId,
            ItemId = itemId
        };
        WebSocketRequest request = new WebSocketRequest {
            LevelUpItem = levelUpItemRequest
        };
        SendWebSocketMessage(request);
        currentMessageHandler = (data) => AwaitItemResponse(data, onItemDataReceived);
        ws.OnMessage += currentMessageHandler;
        ws.OnMessage -= OnWebSocketMessage;
    }

    private void AwaitItemResponse(byte[] data, Action<Item> onCampaignDataReceived)
    {
        try
        {
            WebSocketResponse webSocketResponse = WebSocketResponse.Parser.ParseFrom(data);
            if(webSocketResponse.ResponseTypeCase == WebSocketResponse.ResponseTypeOneofCase.Item) {
                ws.OnMessage -= currentMessageHandler;
                Item item = CreateItemFromData(webSocketResponse.Item);
                onCampaignDataReceived?.Invoke(item);
            }
            else if(webSocketResponse.ResponseTypeCase == WebSocketResponse.ResponseTypeOneofCase.Error) {
                Debug.LogError(webSocketResponse.Error.Reason);
            }
            ws.OnMessage += OnWebSocketMessage;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
