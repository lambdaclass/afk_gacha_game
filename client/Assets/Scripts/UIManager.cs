using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] Transform lobbiesContainer;
    [SerializeField] GameObject lobbyItemPrefab;
    [SerializeField] GameObject gameItemPrefab;
    [SerializeField] Transform gamesContainer;


    bool lobbiesEmpty = true;
    bool gamesEmpty = true;

    // Update is called once per frame
    void Update()
    {
        if (lobbiesEmpty && LobbyConnection.Instance.lobbiesList.Count > 0)
        {
            GenerateLobbiesList();
            lobbiesEmpty = false;
        }
        if (gamesEmpty && LobbyConnection.Instance.gamesList.Count > 0)
        {
            GenerateGamesList();
            gamesEmpty = false;
        }
    }

    public void GenerateLobbiesList()
    {
        LobbyConnection.Instance.lobbiesList.ForEach(el =>
        {
            GameObject lobbyItem = Instantiate(lobbyItemPrefab, lobbiesContainer);
            lobbyItem.GetComponent<LobbyItem>().setId(el);
        });
    }

    public void GenerateGamesList()
    {
        LobbyConnection.Instance.gamesList.ForEach(el =>
        {
            GameObject gameItem = Instantiate(gameItemPrefab, gamesContainer);
            gameItem.GetComponent<GameItem>().setId(el);
        });
    }
}
