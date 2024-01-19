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
    [SerializeField] TextMeshProUGUI gemsRate;
    [SerializeField] TextMeshProUGUI goldRate;
    [SerializeField] TextMeshProUGUI xpRate;

    public void ShowRewards() {
        User user = GlobalUserData.Instance.User;
        user.AccumulateAFKRewards();
        confirmPopUp.SetActive(true);
        gems.text = user.GetCurrencyAfkReward(Currency.Gems).ToString();
        gold.text = user.GetCurrencyAfkReward(Currency.Gold).ToString();
        xp.text = user.accumulatedExperienceReward.ToString();
        gemsRate.text = (user.GetMaxCurrencyReward(Currency.Gems)/720).ToString() + "/m";
        goldRate.text = (user.GetMaxCurrencyReward(Currency.Gold)/720).ToString() + "/m";
        xpRate.text = (user.afkMaxExperienceReward/720).ToString() + "/m";
    }

    public void ClaimRewards() {
        User user = GlobalUserData.Instance.User;
        user.AccumulateAFKRewards();
        user.ClaimAFKRewards();
    }
}
