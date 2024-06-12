using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerSelect : MonoBehaviour
{
    readonly Dictionary<string, string> servers = new Dictionary<string, string>
    {
        {"LOCALHOST", "ws://localhost:4001"},
        {"EUROPE", "wss://central-europe-testing.curseofmirra.com"},
    };

    public static string Name { get; private set; }
    public static string Domain { get; private set; }

    [SerializeField]
    TMP_InputField customServerDomain;

    [SerializeField]
    TMP_Text serverButtonText;

    void Awake()
    {
        if (string.IsNullOrEmpty(ServerSelect.Name) || string.IsNullOrEmpty(ServerSelect.Domain))
        {
#if UNITY_EDITOR
            ServerSelect.Name = "LOCALHOST";
            ServerSelect.Domain = servers["LOCALHOST"];
#else
            ServerSelect.Name = "EUROPE";
            ServerSelect.Domain = servers["EUROPE"];
#endif
        }
        serverButtonText.text = ServerSelect.Name;
    }

    public async void SelectServer(string domainName)
    {
        ServerSelect.Name = domainName;
        ServerSelect.Domain = servers[domainName];
        serverButtonText.text = ServerSelect.Name;
        await SocketConnection.Instance.CloseConnection();
        SocketConnection.Instance.ConnectToSession();
    }

    public async void SelectCustomServer()
    {
        ServerSelect.Name = "CUSTOM";
        ServerSelect.Domain = customServerDomain.text;
        await SocketConnection.Instance.CloseConnection();
        SocketConnection.Instance.ConnectToSession();
    }
}
