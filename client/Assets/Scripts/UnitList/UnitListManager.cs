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
        unitUIItem.GetComponentInChildren<Image>().sprite = unit.character.defaultSprite;
        //unitUIItem.GetComponentInChildren<Image>().border = border(unit.rank.ToString());

    }
}
