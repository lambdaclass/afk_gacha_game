using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineupManager : MonoBehaviour, IUnitPopulator
{
    [SerializeField]
    UnitsUIContainer unitsContainer;

    [SerializeField]
    UnitPosition[] playerUnitPositions;

    // change to centralized way to get Characters, so don't have to assign everytime
    [SerializeField]
    List<Character> characters;

    void Start()
    {
        StartCoroutine(
            BackendConnection.GetAvailableUnits(
                units => {
                    List<Unit> unitList = units.Select(unit => new Unit
                    {
                        unitId = unit.id,
                        level = unit.level,
                        character = characters.Find(character => unit.character.ToLower() == character.name.ToLower()),
                        slot = unit.slot,
                        selected = unit.selected
                    }).ToList();
                    this.unitsContainer.Populate(unitList, this);
                    SetUpSelectedUnits(unitList);
                }
            )
        );

        unitsContainer.OnUnitSelected.AddListener(AddUnitToLineup);
    }

    private void SetUpSelectedUnits(List<Unit> units)
    {
        foreach(Unit unit in units.Where(unit => unit.selected)) {
            UnitPosition unitPosition;
            if(unit.slot.HasValue) {
                unitPosition = playerUnitPositions[unit.slot.Value];
            } else {
                unitPosition = playerUnitPositions.First(position => !position.IsOccupied);
            }
            unitPosition.SetUnit(unit);
            unitPosition.OnUnitRemoved += RemoveUnitFromLineup;
        }
    }

    private void AddUnitToLineup(Unit unit)
    {
        UnitPosition unitPosition = playerUnitPositions.FirstOrDefault(unitPosition => !unitPosition.IsOccupied);

        if(unitPosition)
        {
            int slot = Array.FindIndex(playerUnitPositions, up => !up.IsOccupied);

            StartCoroutine(
                BackendConnection.SelectUnit(unit.unitId, slot)
            );
            unitPosition.SetUnit(unit);
            unitPosition.OnUnitRemoved += RemoveUnitFromLineup;
        }
    }

    private void RemoveUnitFromLineup(Unit unit)
    {
        StartCoroutine(
            BackendConnection.UnselectUnit(unit.unitId)
        );
        unitsContainer.SetUnitUIActiveById(unit.unitId);
    }

    public void Populate(Unit unit, GameObject unitItem)
    {
        var ss = new SpriteState();
        ss.disabledSprite = unit.character.disabledSprite;
        Button unitItemButton = unitItem.GetComponent<Button>();
        unitItemButton.spriteState = ss;
        if(unit.selected) {
            unitItemButton.interactable = false;
        }
    }
}
