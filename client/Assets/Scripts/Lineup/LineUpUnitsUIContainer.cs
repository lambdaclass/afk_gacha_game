using System.Collections.Generic;
using System.Linq;
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
            unitItemButton.onClick.AddListener(() => SelectUnit(unit, unitUIItem));
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
        OnUnitSelected.Invoke(unit);
        Button unitItemButton = selector.GetComponent<Button>();
        unitItemButton.interactable = false;
        SetLocks();
    }

    public void SetUnitUIActiveById(string unitId, bool active)
    {
        unitUIItemDictionary[unitId].GetComponent<Button>().interactable = active;
        unitUIItemDictionary[unitId].GetComponent<UnitItemUI>().SetSelectedChampionMark(!active);
        SetLocks();
    }

    private void SetLocks()
    {
        if (unitUIItemDictionary.Values.Count(unitItem => unitItem.GetComponent<UnitItemUI>().IsSelected()) == 5)
        {
            foreach (GameObject unitUIItem in unitUIItemDictionary.Values)
            {
                if (!unitUIItem.GetComponent<UnitItemUI>().IsSelected())
                {
                    unitUIItem.GetComponent<UnitItemUI>().SetLocked(true);
                }
            }
        }
        else
        {
            foreach (GameObject unitUIItem in unitUIItemDictionary.Values)
            {
                unitUIItem.GetComponent<UnitItemUI>().SetLocked(false);
            }
        }
    }
}
