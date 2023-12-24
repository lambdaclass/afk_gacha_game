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

    public void Populate(List<Unit> units)
    {
        unitsContainer.SetActive(false);
        units.ForEach(unit =>
        {
            GameObject unitItem = Instantiate(unitItemUIPrefab, unitsContainer.transform);
            unitItem.GetComponent<Image>().sprite = unit.character.iconSprite;
            // between here
            var ss = new SpriteState();
            ss.disabledSprite = unit.character.disabledSprite;
            Button unitItemButton = unitItem.GetComponent<Button>();
            unitItemButton.spriteState = ss;
            unitItemButton.onClick.AddListener(() => SelectUnit(unit, unitItemButton));
            if(unit.selected) {
                unitItemButton.interactable = false;
            }
            // and here
        });
        unitsContainer.SetActive(true);
    }

    public void SelectUnit(Unit unit, Button unitItemButton)
    {
        OnUnitSelected.Invoke(unit);
        unitItemButton.interactable = false;
    }
}
