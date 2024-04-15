using System.Linq;
using TMPro;
using UnityEngine;

public class AFKRewardManager : MonoBehaviour
{
    [SerializeField] GameObject confirmPopUp;
    [SerializeField] TextMeshProUGUI gems;
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI xp;

    public void ShowRewards() {
        GlobalUserData user = GlobalUserData.Instance;
        user.AccumulateAFKRewards();
        SocketConnection.Instance.GetAfkRewards(user.User.id, (afkRewards) => {
            confirmPopUp.SetActive(true);
            gems.text = $"{afkRewards.Single(ar => ar.currency == Currency.Gems).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Gems).rate}/m)";
            gold.text = $"{afkRewards.Single(ar => ar.currency == Currency.Gold).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Gold).rate}/m)";
            //xp.text = $"{afkRewards.Single(ar => ar.currency == Currency.Experience).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Experience)}/m)";
        });
    }

    public void ClaimRewards() {
        GlobalUserData user = GlobalUserData.Instance;
        user.AccumulateAFKRewards();
        user.ClaimAFKRewards();
    }
}
