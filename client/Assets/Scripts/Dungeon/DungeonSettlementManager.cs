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
    [SerializeField] GameObject levelUpButton;
    private const int SECONDS_IN_DAY = 86400;

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
                suppliesAfkRewardRate.text = GetAfkRewardRateText(afkRewardRate.rate);
            }
            else
            {
                Debug.LogError($"Unhandled currency: {afkRewardRate.currency}");
            }
        }
    }

    private string GetAfkRewardRateText(float rate)
    {
        return $"{rate * SECONDS_IN_DAY}/day";
    }
}
