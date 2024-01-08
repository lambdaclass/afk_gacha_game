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

    private GlobalUserData globalUserData;
    private OpponentData opponentData;

    void Start()
    {
        globalUserData = GlobalUserData.Instance;
        opponentData = OpponentData.Instance;

        List<Unit> units = globalUserData.Units;
        User opponent = opponentData.User;

        bool won = Battle(units, opponent.units);
        if(won) {
            victorySplash.SetActive(true);
        } else {
            defeatSplash.SetActive(true);
        }
        opponentData.Destroy();
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
}
