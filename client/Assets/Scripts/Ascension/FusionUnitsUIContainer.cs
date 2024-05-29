using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FusionUnitsUIContainer : UnitsUIContainer
{
    public override void Populate(List<Unit> units, IUnitPopulator unitPopulator = null)
    {
        unitsContainer.SetActive(false);
        this.Clear();
        units.ForEach(unit =>
        {
            GameObject unitUIItem = Instantiate(unitItemUIPrefab, unitsContainer.transform);
            unitUIItem.GetComponent<UnitItemUI>().SetUpUnitItemUI(unit);
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

    public override void SelectUnit(Unit unit, GameObject UISelector)
    {
        if (UISelector.GetComponent<UnitItemUI>().IsSelected())
        {
            UISelector.GetComponent<UnitItemUI>().SetSelectedChampionMark(false);
        }
        else
        {
            UISelector.GetComponent<UnitItemUI>().SetSelectedChampionMark(true);
        }
        OnUnitSelected.Invoke(unit);
    }

    public void SetUnitsLock(Faction faction, bool locked)
    {
        // Lock all the units from the faction
        foreach (var unit in unitUIItemDictionary.Values)
        {
            if (unit.GetComponent<UnitItemUI>().IsSelected())
            {
                continue;
            }
            if (unit.GetComponent<UnitItemUI>().GetUnitFaction() != faction.ToString().ToLower())
            {
                unit.GetComponent<UnitItemUI>().SetLocked(locked);
            }
        }
    }
}
