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

    public override void SelectUnit(Unit unit, GameObject unitUIItem)
    {
        Sprite unitUIItemSprite = unitUIItem.GetComponent<Image>().sprite;
        if (unitUIItemSprite == unit.character.defaultSprite)
        {
            unitUIItemSprite = unit.character.selectedSprite;   
        }
        else
        {
            unitUIItemSprite = unit.character.defaultSprite;
        }
        unitUIItem.GetComponent<Image>().sprite = unitUIItemSprite;
        OnUnitSelected.Invoke(unit);
    }
}
