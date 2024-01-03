using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    GameObject victorySplash;

    [SerializeField]
    GameObject defeatSplash;

    string playerDeviceId = "user1";
    string opponentId;

    void Start()
    {
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
                        },
                        error => {
                            Debug.LogError("Error when getting the battle result: " + error);
                        }
                        )
                    );
                },
                error => {
                    Debug.LogError("Error when getting the opponents: " + error);
                }
            )
        );
    }
}
