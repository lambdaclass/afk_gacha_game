using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AFKRewardManager : MonoBehaviour
{
    public void ShowRewards() {
        User user = GlobalUserData.Instance.User;
        user.AccumulateAFKRewards();
    }
}
