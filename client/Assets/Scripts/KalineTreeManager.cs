using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KalineTreeManager : MonoBehaviour
{
    [SerializeField] GameObject confirmPopUp;
    [SerializeField] GameObject insufficientCurrencyPopup;
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
                gems.text = $"{afkRewards.Single(ar => ar.currency == Currency.Gems).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Gems).rate * 60}/m)";
                gold.text = $"{afkRewards.Single(ar => ar.currency == Currency.Gold).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Gold).rate * 60}/m)";
                //xp.text = $"{afkRewards.Single(ar => ar.currency == Currency.Experience).amount.ToString()} ({user.User.afkRewardRates.Single(arr => arr.currency == Currency.Experience)}/m)";
            }
        });
    }

    public void ClaimRewards() {
        SocketConnection.Instance.ClaimAfkRewards(GlobalUserData.Instance.User.id, (user_received) => {
            GlobalUserData user_to_update = GlobalUserData.Instance;
            Dictionary<Currency, int> currencies_to_add = new Dictionary<Currency, int>();

            user_received.currencies.Select(c => c.Key).ToList().ForEach(c => {
                if (!currencies_to_add.ContainsKey(c)) {
                    currencies_to_add.Add(c, user_received.currencies[c] - user_to_update.GetCurrency(c).Value);
                }
            });
            currencies_to_add.Add(Currency.Experience, user_received.experience - user_to_update.User.experience);
            user_to_update.AddCurrencies(currencies_to_add);
        });
        confirmPopUp.SetActive(false);
    }

    public void LevelUpKalineTree()
    {
        SocketConnection.Instance.LevelUpKalineTree(
            GlobalUserData.Instance.User.id, 
            (user_received) => {
                GlobalUserData user_to_update = GlobalUserData.Instance;
                Dictionary<Currency, int> currencies_to_add = new Dictionary<Currency, int>();

                user_received.currencies.Select(c => c.Key).ToList().ForEach(c => {
                    if (!currencies_to_add.ContainsKey(c)) {
                        currencies_to_add.Add(c, user_received.currencies[c] - user_to_update.GetCurrency(c).Value);
                    }
                });
                currencies_to_add.Add(Currency.Experience, user_received.experience - user_to_update.User.experience);
                user_to_update.AddCurrencies(currencies_to_add);
            },
            (reason) => {
                Debug.Log(reason);
                if(reason == "cant_afford") {
                    insufficientCurrencyPopup.SetActive(true);
                }
            }
        );
    }
}
