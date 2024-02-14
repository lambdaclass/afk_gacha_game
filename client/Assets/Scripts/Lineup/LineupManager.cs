using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LineupManager : MonoBehaviour, IUnitPopulator
{
    [SerializeField]
    LineUpUnitsUIContainer unitsContainer;

    [SerializeField]
    UnitPosition[] playerUnitPositions;

    [SerializeField]
    UnitPosition[] opponentUnitPositions;

    [SerializeField]
    List<Character> characters;

    void Start()
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;
        List<Unit> units = globalUserData.Units;

        this.unitsContainer.Populate(units, this);
        SetUpSelectedUnits(units, true);

        unitsContainer.OnUnitSelected.AddListener(AddUnitToLineup);
        
        SetUpSelectedUnits(LevelData.Instance.Units, false);
    }

    private void SetUpSelectedUnits(List<Unit> units, bool isPlayer)
    {
        UnitPosition[] unitPositions = isPlayer ? playerUnitPositions : opponentUnitPositions;
        foreach(Unit unit in units.Where(unit => unit.selected)) {
            UnitPosition unitPosition;
            unitPosition = unitPositions[unit.slot.Value];
            unitPosition.SetUnit(unit, isPlayer);
            unitPosition.OnUnitRemoved += RemoveUnitFromLineup;
        }
    }

    private void AddUnitToLineup(Unit unit)
    {
        UnitPosition unitPosition = playerUnitPositions.First(unitPosition => !unitPosition.IsOccupied);

        if(unitPosition)
        {
            int slot = Array.FindIndex(playerUnitPositions, up => !up.IsOccupied);
            unit.selected = true;
            unit.slot = slot;
            unitPosition.SetUnit(unit, true);
            unitPosition.OnUnitRemoved += RemoveUnitFromLineup;
            unitsContainer.SetUnitUIActiveById(unit.id, false);
        }
    }

    private void RemoveUnitFromLineup(Unit unit)
    {
        UnitPosition unitPosition = playerUnitPositions.First(unitPosition => CompareUnitId(unitPosition, unit));
        unitPosition.OnUnitRemoved -= RemoveUnitFromLineup;
        unit.selected = false;
        unit.slot = null;
        unitsContainer.SetUnitUIActiveById(unit.id, true);
    }

    private bool CompareUnitId(UnitPosition unitPosition, Unit unit)
    {
        Unit selectedUnit = unitPosition.GetSelectedUnit();
        if(selectedUnit != null)
        {
            return selectedUnit.id == unit.id;
        }
        return false;
    }

    public void Populate(Unit unit, GameObject unitItem)
    {
        UnitItemUI unitItemUI = unitItem.GetComponent<UnitItemUI>();
        unitItemUI.SetUpUnitItemUI(unit);
        Button unitItemButton = unitItem.GetComponent<Button>();
        if(unit.selected) {
            unitItemUI.SetSelectedChampionMark(true);
            unitItemButton.interactable = false;
        } else if (playerUnitPositions.Count() >= 5) {
            unitItemUI.SetLocked(true);
            unitItemButton.interactable = false;
        }
    }
}
