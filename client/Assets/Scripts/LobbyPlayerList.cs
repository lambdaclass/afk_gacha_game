using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbyPlayerList : MonoBehaviour
{
    [SerializeField] GameObject playerItemPrefab;
    int totalPlayersBefore = 1;

    // Start is called before the first frame update

    private void CreatePlayerItem()
    {
        GameObject newPlayer = Instantiate(playerItemPrefab, gameObject.transform);
        PlayerItem playerI = newPlayer.GetComponent<PlayerItem>();
        if (LobbyConnection.Instance.playerId == 1)
        {
            playerI.playerText.text += " " + (LobbyConnection.Instance.playerId).ToString() + " " + "HOST";
        }
    }
    void Start()
    {
        CreatePlayerItem();
    }

    // Update is called once per frame
    void Update()
    {
        if (totalPlayersBefore != LobbyConnection.Instance.playerCount)
        {
            CreatePlayerItem();
            totalPlayersBefore++;
        }
    }
}
