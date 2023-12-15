using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UnitList : MonoBehaviour
{
    [SerializeField]
    GameObject UnitItemUIPrefab;

    [SerializeField]
    GameObject UnitListContainer;

    [SerializeField]
    List<Unit> unit;

    [NonSerialized]
    public UnityEvent<Unit> OnUnitSelected = new UnityEvent<Unit>();

    void Start()
    {
        PopulateList();
    }

    private void PopulateList()
    {
        unit.ForEach(unit =>
        {
            GameObject unitItem = Instantiate(UnitItemUIPrefab, UnitListContainer.transform);
            unitItem.GetComponent<Image>().sprite = unit.unitSprite;
            unitItem.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(unit));
        });
    }

    public void SelectCharacter(Unit unit)
    {
        OnUnitSelected.Invoke(unit);
    }
}
