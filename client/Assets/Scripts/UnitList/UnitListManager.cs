using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitListManager : MonoBehaviour
{
    [SerializeField]
    UnitsUIContainer unitsContainer;

    // change to centralized way to get Characters, so don't have to assign everytime
    [SerializeField]
    List<Character> characters;

    void Start() {
        StartCoroutine(
            BackendConnection.GetAvailableUnits(
                "faker_device",
                units => {
                    List<Unit> unitList = units.Select(unit => new Unit
                    {
                        unitId = unit.id,
                        level = unit.level,
                        character = characters.Find(character => unit.character.ToLower() == character.name.ToLower()),
                        slot = unit.slot,
                        selected = unit.selected
                    }).ToList();
                    this.unitsContainer.Populate(unitList);
                }
            )
        );

        unitsContainer.OnUnitSelected.AddListener(SelectUnit);
    }

    void SelectUnit(Unit unit) {
        UnitDetail.SelectUnit(unit);
    }
}
