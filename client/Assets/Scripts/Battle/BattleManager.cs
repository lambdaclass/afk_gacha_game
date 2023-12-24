using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    GameObject victorySplash;

    [SerializeField]
    GameObject defeatSplash;

    void Start()
    {
        string playerDeviceId = "faker_device";
        string opponentId = "31ee2cf1-d718-439c-be35-34bd8bad8781";
        StartCoroutine(
            BackendConnection.GetBattleResult(playerDeviceId, opponentId,
            winnerId => {
                if(winnerId == opponentId) {
                    defeatSplash.SetActive(true);
                } else {
                    victorySplash.SetActive(true);
                }
            })
        );
    }
}
