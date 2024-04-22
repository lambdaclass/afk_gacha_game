using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSelect : MonoBehaviour
{
	#if UNITY_EDITOR
        private const string _defaultServerName = "LOCALHOST";
        private const string _defaultServerIp = "ws://localhost:4001";
    #else
        private const string _defaultServerName = "EUROPE";
        private const string _defaultServerIp = "wss://central-europe-testing.curseofmirra.com/";
    #endif

	
}
