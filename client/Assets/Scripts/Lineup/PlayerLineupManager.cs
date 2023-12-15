using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerLineupManager : MonoBehaviour
{
    [SerializeField]
    UnitList unitList;

    [SerializeField]
    List<UnitPosition> unitPositions;

    void Start() {
        unitList.OnUnitSelected.AddListener(AddUnitToLineup);
    }

    private void AddUnitToLineup(Unit unit)
    {
        UnitPosition up = unitPositions.FirstOrDefault(unitPosition => !unitPosition.IsOccupied);
        if(up)
        {
            up.SetUnit(unit);
        }
    }
}
