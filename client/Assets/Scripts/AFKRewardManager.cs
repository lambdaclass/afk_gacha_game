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
        GlobalUserData user = GlobalUserData.Instance;
        user.AccumulateAFKRewards();
        confirmPopUp.SetActive(true);
        gems.text = user.GetCurrencyAfkReward(Currency.Gems).ToString();
        gold.text = user.GetCurrencyAfkReward(Currency.Gold).ToString();
        xp.text = user.User.accumulatedExperienceReward.ToString();
        gemsRate.text = (user.GetMaxCurrencyReward(Currency.Gems)/720).ToString() + "/m";
        goldRate.text = (user.GetMaxCurrencyReward(Currency.Gold)/720).ToString() + "/m";
        xpRate.text = (user.User.afkMaxExperienceReward/720).ToString() + "/m";
    }

    public void ClaimRewards() {
        GlobalUserData user = GlobalUserData.Instance;
        user.AccumulateAFKRewards();
        user.ClaimAFKRewards();
    }
}
