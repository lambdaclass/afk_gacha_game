using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGameButton : MonoBehaviour
{
    private PlayGameButton playGameButton;

    // Start is called before the first frame update

    void Start()
    {
        if (LobbyConnection.Instance.playerId == 1)
        {
            playGameButton = GetComponent<PlayGameButton>();
            playGameButton.gameObject.SetActive(true);
        }
        else
        {
            playGameButton = GetComponent<PlayGameButton>();
            playGameButton.gameObject.SetActive(false);
        }
    }
}
