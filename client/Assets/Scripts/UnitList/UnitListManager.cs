using System.Collections.Generic;
using System.Linq;
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
        Debug.Log("User items: " + GlobalUserData.Instance.User.items.Count);
        Debug.Log("Last unit weapon: " + GlobalUserData.Instance.Units[GlobalUserData.Instance.Units.Count - 1].weapon);
    }

    void SelectUnit(Unit unit) {
        UnitDetail.SelectUnit(unit);
    }

    public void Populate(Unit unit, GameObject unitUIItem) {
        unitUIItem.GetComponent<Image>().sprite = unit.character.availableSprite;
    }
}
