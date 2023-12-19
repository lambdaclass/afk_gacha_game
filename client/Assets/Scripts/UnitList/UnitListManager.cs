using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// I didn't like the way I solved this problem, this class shouldn't exist, the ideal thing would be for the UnitDetail.SetSelectedUnit static method to be referenced in UnitList (via inspector) and add the listener to unitList.OnUnitSelected there, enabling using the inspector to select whatrever methods you choose to be triggered on click.
public class UnitListManager : MonoBehaviour
{
    [SerializeField]
    UnitList unitList;

    [SerializeField]
    LevelManager levelManager;

    void Start() {
        unitList.OnUnitSelected.AddListener(SelectUnit);
    }

    void SelectUnit(Unit unit) {
        UnitDetail.SelectUnit(unit);
        levelManager.ChangeToScene("UnitDetail");
    }
}
