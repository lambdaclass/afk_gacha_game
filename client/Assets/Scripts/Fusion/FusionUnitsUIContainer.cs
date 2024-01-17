using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FusionUnitsUIContainer : MonoBehaviour
{
    [SerializeField]
    GameObject unitItemUIPrefab;

    [SerializeField]
    GameObject unitsContainer;

    [NonSerialized]
    public UnityEvent<Unit> OnUnitSelected = new UnityEvent<Unit>();

    private Dictionary<string, GameObject> unitUIItemDictionary = new Dictionary<string, GameObject>();

    public void Populate(List<Unit> units, IUnitPopulator unitPopulator = null)
    {
        unitsContainer.SetActive(false);
        this.Clear();
        units.ForEach(unit =>
        {
            GameObject unitUIItem = Instantiate(unitItemUIPrefab, unitsContainer.transform);
            unitUIItem.GetComponent<Image>().sprite = unit.character.availableSprite;
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

    public void Clear()
    {
        foreach (Transform child in unitsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        unitUIItemDictionary.Clear();
    }

    public void SelectUnit(Unit unit, GameObject unitUIItem)
    {
        Sprite unitUIItemSprite = unitUIItem.GetComponent<Image>().sprite;
        if (unitUIItemSprite == unit.character.availableSprite)
        {
            unitUIItemSprite = unit.character.selectedSprite;   
        }
        else
        {
            unitUIItemSprite = unit.character.availableSprite;
        }
        unitUIItem.GetComponent<Image>().sprite = unitUIItemSprite;
        OnUnitSelected.Invoke(unit);
    }

    public void SetUnitUIActiveById(string unitId)
    {
        unitUIItemDictionary[unitId].GetComponent<Button>().interactable = true;
    }
}
