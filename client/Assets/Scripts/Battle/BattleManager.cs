using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
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


    void Start()
    {
        print("Hi");
        print(characters[1]);

        globalUserData = GlobalUserData.Instance;

        User user = globalUserData.User;
        User opponent = GetOpponent();
        bool won = Battle(user.units, opponent.units);
        if(won) {
            victorySplash.SetActive(true);
        } else {
            defeatSplash.SetActive(true);
        }
    }

    // Run a battle between two teams. Returns "Team 1" or "Team 2".
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

    public User GetOpponent()
    {
        User user = new User
        {
            id = "2",
            username = "SampleOpponent",
            units = new List<Unit>
            {
                new Unit { id = "201", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 1, selected = true },
                new Unit { id = "202", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 2, selected = true },
                new Unit { id = "203", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 3, selected = true },
                new Unit { id = "204", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 4, selected = true },
                new Unit { id = "205", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 5, selected = true }
            }
        };

        return user;
    }
}
