using System;
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

    [SerializeField]
    UnitPosition[] opponentUnitPositions;

    [SerializeField]
    List<Character> characters;

    void Start()
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;
        OpponentData opponentData = OpponentData.Instance;

        List<Unit> units = globalUserData.Units;

        this.unitsContainer.Populate(units, this);
        SetUpSelectedUnits(units, true);

        unitsContainer.OnUnitSelected.AddListener(AddUnitToLineup);

        User user2 = GetOpponent();
        opponentData.User = user2;
        SetUpSelectedUnits(user2.units, false);
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

    public User GetOpponent()
    {
        User user = new User
        {
            username = "SampleOpponent",
            units = new List<Unit>
            {
                new Unit { id = "201", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 0, selected = true },
                new Unit { id = "202", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 1, selected = true },
                new Unit { id = "203", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 2, selected = true },
                new Unit { id = "204", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 3, selected = true },
                new Unit { id = "205", level = 5, character = characters.Find(character => "h4ck" == character.name.ToLower()), slot = 4, selected = true }
            }
        };

        return user;
    }
}
