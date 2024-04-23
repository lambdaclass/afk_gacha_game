using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ServerSelect : MonoBehaviour
{
	#if UNITY_EDITOR
        private const string _defaultServerName = "LOCALHOST";
        private const string _defaultServerDomain = "ws://localhost:4001";
    #else
        private const string _defaultServerName = "EUROPE";
        private const string _defaultServerDomain = "wss://central-europe-testing.curseofmirra.com";
    #endif

	public static string Domain { get; private set; }
	public static string Name { get; private set; }

	[SerializeField]
	SocketConnection socketConnection;

	[SerializeField]
	TMP_InputField serverAdress;

	[SerializeField]
	TMP_Text serverButtonText;

	void Start() {
		ServerSelect.Domain = _defaultServerDomain;
	}

	public async void SelectServer(string domainName) {
		ServerSelect.Domain = domainName;
		await socketConnection.CloseConnection();
		await socketConnection.Init();
	}
}
