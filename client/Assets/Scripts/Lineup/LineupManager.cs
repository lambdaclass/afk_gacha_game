using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineupManager : MonoBehaviour
{
    [SerializeField]
    CharacterList characterList;

    [SerializeField]
    UnitPosition[] playerUnitPositions;

    [SerializeField]
    List<Character> characters;

    void Start()
    {
        StartCoroutine(
            BackendConnection.GetAvailableUnits(
                units => {
                    List<Unit> unitList = units.Select(unit => new Unit
                    {
                        unit_id = unit.unit_id,
                        level = unit.level,
                        character = characters.Find(character => unit.character.ToLower() == character.name.ToLower()),
                        slot = unit.slot,
                        selected = unit.selected
                    }).ToList();
                    characterList.PopulateList(unitList);
                    SetUpSelectedUnits(unitList);
                }
            )
        );

        characterList.OnCharacterSelected.AddListener(AddUnitToLineup);
    }

    private void SetUpSelectedUnits(List<Unit> units)
    {
        foreach(Unit unit in units.Where(unit => unit.selected)) {
            UnitPosition unitPosition;
            if(unit.slot.HasValue) {
                print($"unit position: {unit.slot.Value}");
                unitPosition = playerUnitPositions[unit.slot.Value];
            } else {
                unitPosition = playerUnitPositions.First(position => !position.IsOccupied);
            }
            unitPosition.SetUnit(unit);
        }
    }

    private void AddUnitToLineup(Unit unit)
    {
        UnitPosition unitPosition = playerUnitPositions.FirstOrDefault(unitPosition => !unitPosition.IsOccupied);

        if(unitPosition)
        {
            unitPosition.SetUnit(unit);
        }
    }
}
