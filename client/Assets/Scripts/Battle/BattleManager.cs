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

    [SerializeField]
    List<Character> characters;

    void Start()
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;
        LevelItem levelData = LevelItem.Instance;

        User user = globalUserData.User;

        List<Unit> userUnits = globalUserData.Units;
        List<Unit> opponentUnits = levelData.Units;

        SetUpUnits(userUnits, opponentUnits);

        bool won = Battle(userUnits, opponentUnits);
        if(won) {
            user.AddCurrency(levelData.Rewards);
            user.AddExperience(levelData.Experience);
            user.AccumulateAFKRewards();
            user.afkMaxCurrencyReward = levelData.AfkCurrencyRate;
            user.afkMaxExperienceReward = levelData.AfkExperienceRate;
            LevelProgressData.Instance.ProcessLevelCompleted();
            CampaignProgressData.Instance.ProcessLevelCompleted();
            victorySplash.GetComponentInChildren<RewardsUIContainer>().Populate(CreateRewardsList());
            victorySplash.SetActive(true);
            victorySplash.GetComponent<AudioSource>().Play();

            GameObject nextButton = victorySplash.transform.Find("Next").gameObject;
            GameObject victoryText = victorySplash.transform.Find("CenterContainer").transform.Find("Sign").transform.Find("Text").gameObject;

            if (levelData.CampaignToComplete == "") {
                nextButton.GetComponentInChildren<Text>().text = "NEXT STAGE";
                victoryText.GetComponent<Text>().text = "Victory!";
            } else {
                // We assume this level's campaign is the one that was completed
                victoryText.GetComponent<Text>().text = "Campaign beaten!";
                if (levelData.CampaignToUnlock == "") {
                    // No "next campaign". Just go back to CampaignsMap.
                    nextButton.SetActive(false);
                } else {
                    nextButton.GetComponentInChildren<Text>().text = "BACK TO CAMPAIGNS";
                }
            }
        } else {
            defeatSplash.SetActive(true);
            defeatSplash.GetComponent<AudioSource>().Play();
        }
    }

    // Run a battle between two teams. Returns true if our user wins
    public bool Battle(List<Unit> team1, List<Unit> team2)
    {
        int team1AggLevel = CalculateAggregateLevel(team1);
        int team2AggLevel = CalculateAggregateLevel(team2);
        int totalLevel = team1AggLevel + team2AggLevel;

        return UnityEngine.Random.Range(1, totalLevel + 1) <= team1AggLevel;
    }

    // Helper method to calculate the aggregate level of a team
    private int CalculateAggregateLevel(List<Unit> team)
    {
        return team.Sum(unit => unit.level);
    }

    private void SetUpUnits(List<Unit> userUnits, List<Unit> opponentUnits)
    {
        SetUpUserUnits(userUnits, true);
        SetUpUserUnits(opponentUnits, false);
    }

    private void SetUpUserUnits(List<Unit> units, bool isPlayer)
    {
        UnitPosition[] unitPositions = isPlayer ? playerUnitPositions : opponentUnitPositions;
        foreach(Unit unit in units.Where(unit => unit.selected && unit.slot.Value < unitPositions.Length)) {
            UnitPosition unitPosition;
            unitPosition = unitPositions[unit.slot.Value];
            unitPosition.SetUnit(unit, isPlayer);    
        }
    }

    private List<UIReward> CreateRewardsList() {
        LevelItem levelData = LevelItem.Instance;

        List<UIReward> rewards = new List<UIReward>();

        if (levelData.Experience != 0) { rewards.Add(new ExperienceUIReward(levelData.Experience)); }

        foreach (var currencyReward in levelData.Rewards) {
            rewards.Add(new CurrencyUIReward(currencyReward.Key, currencyReward.Value));
        }

        return rewards;
    }

    public void Next() {
        LevelItem levelData = LevelItem.Instance;

        // If nextLevel is null this won't do anything
        if (levelData.CampaignToUnlock != "") {
            gameObject.GetComponent<LevelManager>().ChangeToScene("CampaignsMap");
        } else {
            CampaignManager.automaticLoadLevelName = levelData.NextLevelName;
            gameObject.GetComponent<LevelManager>().ChangeToScene("Campaign");
        }
    }
}
