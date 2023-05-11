using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfo : MonoBehaviour
{
    [SerializeField] Text lobbyID;
    [SerializeField] Text playersAmount;
    // Start is called before the first frame update
    void Start()
    {
        lobbyID.text = LobbyConnection.Instance.LobbySession;
        playersAmount.text += " " + (LobbyConnection.Instance.playerCount).ToString();
    }

    // Update is called once per frame
    void Update()
    {
        print(Int32.Parse(((playersAmount.text).Split(": ")[1])));
        if (Int32.Parse(((playersAmount.text).Split(": ")[1])) < LobbyConnection.Instance.playerCount)
        {
            playersAmount.text += " " + (LobbyConnection.Instance.playerCount).ToString();
        }
    }
}
