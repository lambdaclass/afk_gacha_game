using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitPosition : MonoBehaviour
{
    [SerializeField]
    TMP_Text unitName;
    
    [SerializeField]
    GameObject removeSign;

    [SerializeField]
    GameObject modelContainer;

    public event Action<Unit> OnUnitRemoved;

    private bool isOccupied;
    public bool IsOccupied => isOccupied;

    private Unit selectedUnit;

    public void SetUnit(Unit unit, bool isPlayer) {
        selectedUnit = unit;
        unitName.text = $"Lvl: {unit.level}";
        isOccupied = true;
        unitName.gameObject.SetActive(true);
        removeSign.SetActive(isPlayer);
        GetComponent<Button>().interactable = isPlayer;
        Instantiate(unit.character.prefab, modelContainer.transform);
    }

    public void UnselectUnit() {
        unitName.text = String.Empty;
        isOccupied = false;
        unitName.gameObject.SetActive(false);
        removeSign.SetActive(false);
        Destroy(modelContainer.transform.GetChild(0).gameObject);
        GetComponent<Button>().interactable = false;
        OnUnitRemoved?.Invoke(selectedUnit);
        selectedUnit = null;
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }
}
