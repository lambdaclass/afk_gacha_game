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
    Image unitImage;

    public event Action<Unit> OnUnitRemoved;

    private bool isOccupied;
    public bool IsOccupied => isOccupied;

    private Unit selectedUnit;

    public void SetUnit(Unit unit, bool isPlayer) {
        selectedUnit = unit;
        // Uncomment to display unit name and level
        // unitName.text = $"{unit.character.name} LVL: {unit.level}";
        // unitName.transform.parent.gameObject.SetActive(true);
        isOccupied = true;

		// Uncomment to show the remove unit sign
        // removeSign.SetActive(isPlayer);
		
        GetComponent<Button>().interactable = isPlayer;

        // Instantiate(unit.character.prefab, modelContainer.transform);
        unitImage.sprite = selectedUnit.character.inGameSprite;
        unitImage.gameObject.SetActive(true);
    }

    public void UnselectUnit() {
        // Uncomment to display unit name and level
        // unitName.transform.parent.gameObject.SetActive(false);
        // unitName.text = String.Empty;
        isOccupied = false;

		// Uncomment to show the remove unit sign
        // removeSign.SetActive(false);

        unitImage.gameObject.SetActive(false);
        unitImage.sprite = null;
        GetComponent<Button>().interactable = false;
        OnUnitRemoved?.Invoke(selectedUnit);
        selectedUnit = null;
    }

    public Unit GetSelectedUnit() {
        return selectedUnit;
    }
}
