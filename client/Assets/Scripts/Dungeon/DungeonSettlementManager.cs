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
    [SerializeField] GameObject confirmPopUp;
    [SerializeField] GameObject insufficientCurrencyPopup;
    [SerializeField] GameObject afkRewardDetailUI;
    [SerializeField] GameObject afkRewardsContainer;
    private const int SECONDS_IN_DAY = 86400;

    void Start()
    {
        GlobalUserData user = GlobalUserData.Instance;
        goldLevelUpCost.text = user.User.dungeonSettlementLevel.levelUpCosts.Single(cost => cost.currency.name == "Gold").amount.ToString();
        blueprintsLevelUpCost.text = user.User.dungeonSettlementLevel.levelUpCosts.Single(cost => cost.currency.name == "Blueprints").amount.ToString();
        dungeonSettlementLevel.text = $"Level {user.User.dungeonSettlementLevel.level}";
        SetAfkRewardRatesTexts(user.User);
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
                UpdateRatesAndLevelUpCosts(userReceived);
                dungeonSettlementLevel.text = $"Level {userReceived.dungeonSettlementLevel.level}";
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

    public void UpdateRatesAndLevelUpCosts(User user)
    {
        goldLevelUpCost.text = user.dungeonSettlementLevel.levelUpCosts.Single(cost => cost.currency.name == "Gold").amount.ToString();
        blueprintsLevelUpCost.text = user.dungeonSettlementLevel.levelUpCosts.Single(cost => cost.currency.name == "Blueprints").amount.ToString();
        SetAfkRewardRatesTexts(user);
    }

    private void SetAfkRewardRatesTexts(User user)
    {
        foreach (AfkRewardRate afkRewardRate in user.dungeonSettlementLevel.afkRewardRates)
        {
            switch (afkRewardRate.currency)
            {
                case "Supplies":
                    suppliesAfkRewardRate.text = GetAfkRewardRateText(afkRewardRate.rate);
                    break;
            }
        }
    }

    private string GetAfkRewardRateText(float rate)
    {
        return $"{rate * SECONDS_IN_DAY}/day";
    }
}
