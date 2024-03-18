using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class AFKRewardManager : MonoBehaviour
{
    [SerializeField] GameObject confirmPopUp;
    [SerializeField] TextMeshProUGUI gems;
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI xp;

    public void ShowRewards() {
        GlobalUserData user = GlobalUserData.Instance;
        user.AccumulateAFKRewards();
        confirmPopUp.SetActive(true);
        gems.text = $"{user.GetCurrencyAfkReward(Currency.Gems).ToString()} ({(user.GetMaxCurrencyReward(Currency.Gems)/720).ToString()}/m)";
		gold.text = $"{user.GetCurrencyAfkReward(Currency.Gold).ToString()} ({(user.GetMaxCurrencyReward(Currency.Gold)/720).ToString()}/m)";
		xp.text = $"{user.User.accumulatedExperienceReward.ToString()} ({(user.User.afkMaxExperienceReward/720).ToString()}/m)";
    }

    public void ClaimRewards() {
        GlobalUserData user = GlobalUserData.Instance;
        user.AccumulateAFKRewards();
        user.ClaimAFKRewards();
    }
}
