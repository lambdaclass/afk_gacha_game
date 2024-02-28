using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField]
    GameObject victorySplash;

    [SerializeField]
    GameObject defeatSplash;

    [SerializeField]
    UnitPosition[] playerUnitPositions;

    [SerializeField]
    UnitPosition[] opponentUnitPositions;

    public static LevelData selectedLevelData;

    void Start()
    {
        List<Unit> userUnits = GlobalUserData.Instance.Units;
        List<Unit> opponentUnits = selectedLevelData.units;

        SetUpUnits(userUnits, opponentUnits);

        Battle();
    }

    public void Battle()
    {
        SocketConnection.Instance.Battle(GlobalUserData.Instance.User.id, selectedLevelData.id, (result) => {
            HandleBattleResult(result);
        });
    }

    private void HandleBattleResult(bool result)
    {
        if(result) {
            User user = GlobalUserData.Instance.User;
            user.AddCurrency(selectedLevelData.rewards);
            user.AddExperience(selectedLevelData.experienceReward);
            user.AccumulateAFKRewards();
            user.afkMaxCurrencyReward = selectedLevelData.afkCurrencyRate;
            user.afkMaxExperienceReward = selectedLevelData.afkExperienceRate;
            LevelProgressData.Instance.ProcessLevelCompleted();
            // CampaignProgressData.Instance.ProcessLevelCompleted();
            victorySplash.GetComponentInChildren<RewardsUIContainer>().Populate(CreateRewardsList());
            victorySplash.SetActive(true);
            victorySplash.GetComponent<AudioSource>().Play();

            GameObject nextButton = victorySplash.transform.Find("Next").gameObject;
            GameObject victoryText = victorySplash.transform.Find("CenterContainer").transform.Find("Sign").transform.Find("Text").gameObject;

            // Always shows the same texts when a win is achieved, but they should change based on if it was the last level of the campaign and if there are other campaigns after that
            nextButton.GetComponentInChildren<Text>().text = "NEXT STAGE";
            victoryText.GetComponent<Text>().text = "Victory!";

            // if (selectedLevelData.campaignToComplete == "") {
            //     nextButton.GetComponentInChildren<Text>().text = "NEXT STAGE";
            //     victoryText.GetComponent<Text>().text = "Victory!";
            // } else {
            //     // We assume this level's campaign is the one that was completed
            //     victoryText.GetComponent<Text>().text = "Campaign beaten!";
            //     if (selectedLevelData.campaignToUnlock == "") {
            //         // No "next campaign". Just go back to CampaignsMap.
            //         nextButton.SetActive(false);
            //     } else {
            //         nextButton.GetComponentInChildren<Text>().text = "BACK TO CAMPAIGNS";
            //     }
            // }
        } else {
            defeatSplash.SetActive(true);
            defeatSplash.GetComponent<AudioSource>().Play();
        }
    }

    private void SetUpUnits(List<Unit> userUnits, List<Unit> opponentUnits)
    {
        SetUpUserUnits(userUnits, true);
        SetUpUserUnits(opponentUnits, false);
    }

    private void SetUpUserUnits(List<Unit> units, bool isPlayer)
    {
        UnitPosition[] unitPositions = isPlayer ? playerUnitPositions : opponentUnitPositions;
        // The -1 are since the indexes of the slots in the database go from 1 to 6, and the indexes of the unit position game objects range from 0 to 5
        foreach(Unit unit in units.Where(unit => unit.selected && unit.slot.Value - 1 < unitPositions.Length)) {
            UnitPosition unitPosition = unitPositions[unit.slot.Value - 1];
            unitPosition.SetUnit(unit, isPlayer);    
        }
    }

    private List<UIReward> CreateRewardsList() {
        List<UIReward> rewards = new List<UIReward>();

        if (selectedLevelData.experienceReward != 0) { rewards.Add(new ExperienceUIReward(selectedLevelData.experienceReward)); }

        foreach (var currencyReward in selectedLevelData.rewards) {
            rewards.Add(new CurrencyUIReward(currencyReward.Key, currencyReward.Value));
        }

        return rewards;
    }

    public void Next() {
        gameObject.GetComponent<LevelManager>().ChangeToScene("CampaignsMap");

        // If nextLevel is null this won't do anything
        // if (selectedLevelData.campaignToUnlock != "") {
        //     gameObject.GetComponent<SceneManager>().ChangeToScene("CampaignsMap");
        // } else {
        //     CampaignManager.automaticLoadLevelName = selectedLevelData.nextLevel.name;
        //     gameObject.GetComponent<SceneManager>().ChangeToScene("Campaign");
        // }
    }
}
