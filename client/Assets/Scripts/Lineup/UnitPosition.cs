using UnityEngine;
using TMPro;
using System;

public class UnitPosition : MonoBehaviour
{
    [SerializeField]
    TMP_Text unitName;

    [SerializeField]
    UIModelManager UIModelManager;

    private bool isOccupied;
    public bool IsOccupied => isOccupied;

    public void SetUnit(Unit unit) {
        unitName.text = unit.character.name;
        isOccupied = true;
        unitName.gameObject.SetActive(true);
        UIModelManager.SetModel(unit.character.prefab);
    }
}
