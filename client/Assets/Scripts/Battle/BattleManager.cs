using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    GameObject victorySplash;

    [SerializeField]
    GameObject defeatSplash;

    string playerDeviceId = "faker_device";
    string opponentId;

    void Start()
    {
         
        // string opponentId = BackendConnection.GetOpponents(playerDeviceId, opponents => {
        //     opponents[0].id;
        // });
        StartCoroutine(
            BackendConnection.GetOpponents
            (
                playerDeviceId, opponents => {
                    this.opponentId = opponents[0].id;
                    StartCoroutine(
                        BackendConnection.GetBattleResult(playerDeviceId, this.opponentId,
                        winnerId => {
                            if(winnerId == opponentId) {
                                defeatSplash.SetActive(true);
                            } else {
                                victorySplash.SetActive(true);
                            }
                        })
                    );
                }
            )
        );
    }
}
