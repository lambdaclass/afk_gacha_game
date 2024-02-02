using System;
using System.Collections;
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
        // GlobalUserData globalUserData = GlobalUserData.Instance;


        StartCoroutine(GetUser());

        // List<Unit> units = globalUserData.Units;

        // this.unitsContainer.Populate(units, this);
        // SetUpSelectedUnits(units, true);

        // unitsContainer.OnUnitSelected.AddListener(AddUnitToLineup);
        
        // SetUpSelectedUnits(LevelData.Instance.Units, false);
    }

    private IEnumerator GetUser()
    {
        print("start get user");
        print(GlobalUserData.Instance.User.username);
        SocketConnection.Instance.GetUser();
        yield return new WaitUntil(() => GlobalUserData.Instance != null);
        print("finish get user");

        List<Unit> units = GlobalUserData.Instance.Units;

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
        }
    }

    private void RemoveUnitFromLineup(Unit unit)
    {
        UnitPosition unitPosition = playerUnitPositions.First(unitPosition => CompareUnitId(unitPosition, unit));
        unitPosition.OnUnitRemoved -= RemoveUnitFromLineup;
        unit.selected = false;
        unit.slot = null;
        unitsContainer.SetUnitUIActiveById(unit.id);
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
        SpriteState ss = new SpriteState();
        ss.disabledSprite = unit.character.disabledSprite;
        Button unitItemButton = unitItem.GetComponent<Button>();
        unitItemButton.spriteState = ss;
        if(unit.selected) {
            unitItemButton.interactable = false;
        }
    }
}
