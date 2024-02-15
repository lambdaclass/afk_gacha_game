using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitListManager : MonoBehaviour, IUnitPopulator
{
    [SerializeField]
    UnitsUIContainer unitsContainer;

    [SerializeField]
    List<Character> characters;

    void Start() {
        this.unitsContainer.Populate(GlobalUserData.Instance.Units, this);
        unitsContainer.OnUnitSelected.AddListener(SelectUnit);
    }

    void SelectUnit(Unit unit) {
        UnitDetail.SelectUnit(unit);
    }

    public void Populate(Unit unit, GameObject unitUIItem) {
        UnitItemUI unitItemUI = unitUIItem.GetComponent<UnitItemUI>();
        unitItemUI.SetUpUnitItemUI(unit);
        Button unitItemButton = unitUIItem.GetComponent<Button>();
    }
}
