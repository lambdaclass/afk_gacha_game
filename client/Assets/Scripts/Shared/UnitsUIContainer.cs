using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class UnitsUIContainer : MonoBehaviour
{
    [SerializeField]
    protected GameObject unitItemUIPrefab;

    [SerializeField]
    protected GameObject unitsContainer;

    [NonSerialized]
    public UnityEvent<Unit> OnUnitSelected = new UnityEvent<Unit>();

    protected Dictionary<string, GameObject> unitUIItemDictionary = new Dictionary<string, GameObject>();

    public abstract void Populate(List<Unit> units, IUnitPopulator unitPopulator = null);

    public abstract void SelectUnit(Unit unit, GameObject UISelector);
    public void Clear()
    {
        foreach (Transform child in unitsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        unitUIItemDictionary.Clear();
    }
}
