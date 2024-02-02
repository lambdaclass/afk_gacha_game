using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LineUpUnitsUIContainer : UnitsUIContainer
{
    public override void Populate(List<Unit> units, IUnitPopulator unitPopulator = null)
    {
        unitsContainer.SetActive(false);
        this.Clear();
        units.ForEach(unit =>
        {
            GameObject unitUIItem = Instantiate(unitItemUIPrefab, unitsContainer.transform);
            unitUIItem.GetComponentInChildren<Image>().sprite = unit.character.defaultSprite;
            Button unitItemButton = unitUIItem.GetComponent<Button>();
            unitItemButton.onClick.AddListener(() => SelectUnit(unit, unitItemButton.gameObject));
            if (unitPopulator != null)
            {
                unitPopulator.Populate(unit, unitUIItem);
            }
            unitUIItemDictionary.Add(unit.id, unitUIItem);
        });
        unitsContainer.SetActive(true);
    }

    public override void SelectUnit(Unit unit, GameObject selector)
    {
        Button unitItemButton = selector.GetComponent<Button>();
        OnUnitSelected.Invoke(unit);
        unitItemButton.interactable = false;
    }

    public void SetUnitUIActiveById(string unitId)
    {
        unitUIItemDictionary[unitId].GetComponent<Button>().interactable = true;
    }
}
