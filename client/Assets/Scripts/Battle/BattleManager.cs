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
        LevelData levelData = LevelData.Instance;

        User user = globalUserData.User;

        List<Unit> userUnits = globalUserData.SelectedUnits;
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
            victorySplash.SetActive(true);
            victorySplash.GetComponent<AudioSource>().Play();
        } else {
            defeatSplash.SetActive(true);
            defeatSplash.GetComponent<AudioSource>().Play();
        }
        levelData.Destroy();
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
        return team.Sum(unit => unit.CalculateLevel());
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
}
