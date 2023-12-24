using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitsUIContainer : MonoBehaviour
{
    [SerializeField]
    GameObject unitItemUIPrefab;

    [SerializeField]
    GameObject unitsContainer;

    [NonSerialized]
    public UnityEvent<Unit> OnUnitSelected = new UnityEvent<Unit>();

    public void Populate(List<Unit> units, IUnitPopulator unitPopulator = null)
    {
        unitsContainer.SetActive(false);
        units.ForEach(unit =>
        {
            GameObject unitItem = Instantiate(unitItemUIPrefab, unitsContainer.transform);
            unitItem.GetComponent<Image>().sprite = unit.character.iconSprite;
            Button unitItemButton = unitItem.GetComponent<Button>();
            unitItemButton.onClick.AddListener(() => SelectUnit(unit, unitItemButton));
            if (unitPopulator != null)
            {
                unitPopulator.Populate(unit, unitItem);
            }
        });
        unitsContainer.SetActive(true);
    }

    public void SelectUnit(Unit unit, Button unitItemButton)
    {
        OnUnitSelected.Invoke(unit);
        unitItemButton.interactable = false;
    }
}

public interface IUnitPopulator
{
    void Populate(Unit unit, GameObject unitItem);
}
