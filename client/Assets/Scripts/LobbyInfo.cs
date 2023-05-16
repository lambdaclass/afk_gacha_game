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
        var aux = ((playersAmount.text).Trim().Split(": "));
        if (LobbyConnection.Instance.playerCount > int.Parse((aux[1]).Trim()))
        {
            playersAmount.text = "Amount of Players: " + (LobbyConnection.Instance.playerCount).ToString();
        }
    }
}
