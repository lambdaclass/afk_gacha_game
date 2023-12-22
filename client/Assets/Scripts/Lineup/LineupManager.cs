using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineupManager : MonoBehaviour
{
    [SerializeField]
    CharacterList characterList;

    [SerializeField]
    List<UnitPosition> playerUnitPositions;

    void Start()
    {
        StartCoroutine(
            BackendConnection.GetAvailableUnits(
                units => {
                    characterList.PopulateList(units);
                }
            )
        );

        characterList.OnCharacterSelected.AddListener(AddCharacterToLineup);
    }

    private void AddCharacterToLineup(Character character)
    {
        UnitPosition up = playerUnitPositions.FirstOrDefault(unitPosition => !unitPosition.IsOccupied);
        if(up)
        {
            up.SetCharacter(character);
        }
    }
}
