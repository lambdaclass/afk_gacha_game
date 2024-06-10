using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class DungeonSettlementManager : MonoBehaviour
{
    [SerializeField] TMP_Text goldLevelUpCost;
    [SerializeField] TMP_Text blueprintsLevelUpCost;
    [SerializeField] TMP_Text suppliesAfkRewardRate;
    [SerializeField] TMP_Text dungeonSettlementLevel;
    [SerializeField] GameObject afkRewardsPopUp;
    [SerializeField] GameObject insufficientCurrencyPopup;
    [SerializeField] GameObject levelUpButton;
    [SerializeField] GameObject afkRewardDetailUI;
    [SerializeField] GameObject afkRewardsContainer;

    void Start()
    {
        GlobalUserData user = GlobalUserData.Instance;
        SetSceneTexts(user.User);
    }

    public void LevelUpDungeonSettlement()
    {
        SocketConnection.Instance.LevelUpDungeonSettlement(
            GlobalUserData.Instance.User.id,
            (userReceived) =>
            {
                GlobalUserData userToUpdate = GlobalUserData.Instance;
                Dictionary<string, int> currenciesToAdd = new Dictionary<string, int>();

                userReceived.currencies.Select(c => c.Key).ToList().ForEach(c =>
                {
                    if (!currenciesToAdd.ContainsKey(c))
                    {
                        userToUpdate.SetCurrencyAmount(c, userReceived.currencies[c]);
                    }
                });
                userReceived.dungeonSettlementLevel.afkRewardRates.ForEach(afkRewardRate =>
                {
                    userToUpdate.User.dungeonSettlementLevel.afkRewardRates.Add(afkRewardRate);
                });
                SetSceneTexts(userReceived);
            },
            (reason) =>
            {
                if (reason == "cant_afford")
                {
                    insufficientCurrencyPopup.SetActive(true);
                }
            }
        );
    }

    private void SetSceneTexts(User user)
    {

        if (user.dungeonSettlementLevel.levelUpCosts.Any())
        {
            goldLevelUpCost.text = user.dungeonSettlementLevel.levelUpCosts.Single(cost => cost.currency.name == "Gold").amount.ToString();
            blueprintsLevelUpCost.text = user.dungeonSettlementLevel.levelUpCosts.Single(cost => cost.currency.name == "Blueprints").amount.ToString();
        }
        else
        {
            levelUpButton.SetActive(false);
            goldLevelUpCost.transform.parent.gameObject.SetActive(false);
            blueprintsLevelUpCost.transform.parent.gameObject.SetActive(false);
        }
        dungeonSettlementLevel.text = $"Level {user.dungeonSettlementLevel.level}";
        SetAfkRewardRatesTexts(user);
    }

    private void SetAfkRewardRatesTexts(User user)
    {
        foreach (AfkRewardRate afkRewardRate in user.dungeonSettlementLevel.afkRewardRates)
        {
            if (afkRewardRate.currency == "Supplies")
            {
                suppliesAfkRewardRate.text = GetAfkRewardRateText(afkRewardRate.daily_rate);
            }
            else
            {
                Debug.LogError($"Unhandled currency: {afkRewardRate.currency}");
            }
        }
    }

    public void ShowRewards()
    {
        GlobalUserData user = GlobalUserData.Instance;
        SocketConnection.Instance.GetDungeonAfkRewards(user.User.id, (afkRewards) =>
        {
            foreach (Transform child in afkRewardsContainer.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var afkReward in afkRewards.Where(reward => user.User.dungeonSettlementLevel.afkRewardRates.Any(rewardRate => rewardRate.daily_rate > 0 && rewardRate.currency == reward.currency)))
            {
                GameObject afkRewardGO = Instantiate(afkRewardDetailUI, afkRewardsContainer.transform);
                AfkRewardDetail afkRewardDetail = afkRewardGO.GetComponent<AfkRewardDetail>();
                afkRewardDetail.SetData(GlobalUserData.Instance.AvailableCurrencies.Single(currency => currency.name == afkReward.currency).image, $"{afkReward.amount} ({user.User.dungeonSettlementLevel.afkRewardRates.Single(arr => arr.currency == afkReward.currency).daily_rate}/day)");
            }

            afkRewardsPopUp.SetActive(true);
        });
    }

    public void ClaimRewards()
    {
        SocketConnection.Instance.ClaimDungeonAfkRewards(GlobalUserData.Instance.User.id, (userReceived) =>
        {
            GlobalUserData userToUpdate = GlobalUserData.Instance;
            Dictionary<string, int> currenciesToAdd = new Dictionary<string, int>();

            userReceived.currencies.Select(c => c.Key).ToList().ForEach(c =>
            {
                if (!currenciesToAdd.ContainsKey(c))
                {
                    currenciesToAdd.Add(c, userReceived.currencies[c] - userToUpdate.GetCurrency(c).Value);
                }
            });
            userToUpdate.AddCurrencies(currenciesToAdd);
        });
        afkRewardsPopUp.SetActive(false);
    }

    private string GetAfkRewardRateText(float daily_rate)
    {
        return $"{daily_rate}/day";
    }
}
