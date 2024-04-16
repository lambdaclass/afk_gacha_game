using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class AFKRewardManager : MonoBehaviour
{
    [SerializeField] GameObject confirmPopUp;
    [SerializeField] TextMeshProUGUI gems;
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI xp;

    private const string EMPTY_AFK_REWARD = "0 (0/m)";
    public void ShowRewards() {
        GlobalUserData user = GlobalUserData.Instance;
        SocketConnection.Instance.GetAfkRewards(user.User.id, (afkRewards) => {
            confirmPopUp.SetActive(true);
            if (afkRewards.Count == 0) {
                gems.text = EMPTY_AFK_REWARD;
                gold.text = EMPTY_AFK_REWARD;
                xp.text = EMPTY_AFK_REWARD;
            } else {
                gems.text = $"{afkRewards.Single(ar => ar.currency == Currency.Gems).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Gems).rate}/m)";
                gold.text = $"{afkRewards.Single(ar => ar.currency == Currency.Gold).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Gold).rate}/m)";
                //xp.text = $"{afkRewards.Single(ar => ar.currency == Currency.Experience).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Experience)}/m)";
            }
        });
    }

    public void ClaimRewards() {
        int current_gold = GlobalUserData.Instance.GetCurrency(Currency.Gold).Value;
        int current_gems = GlobalUserData.Instance.GetCurrency(Currency.Gems).Value;
        int current_experience = GlobalUserData.Instance.User.experience;

        SocketConnection.Instance.ClaimAfkRewards(GlobalUserData.Instance.User.id, (user_received) => {
            GlobalUserData user_to_update = GlobalUserData.Instance;
            Dictionary<Currency, int> currencies_to_add = new Dictionary<Currency, int>();
            currencies_to_add.Add(Currency.Gold, user_received.currencies[Currency.Gold] - current_gold);
            currencies_to_add.Add(Currency.Gems, user_received.currencies[Currency.Gems] - current_gems);
            user_to_update.AddCurrency(currencies_to_add);
            user_to_update.AddExperience(user_received.experience - current_experience);
        });
        confirmPopUp.SetActive(false);
    }
}
