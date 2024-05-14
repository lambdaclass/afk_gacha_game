using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitPosition : MonoBehaviour
{
    [SerializeField]
    TMP_Text unitNameText;

	[SerializeField]
    TMP_Text unitLevelText;
    
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

        unitNameText.text = $"{unit.character.name}";
        unitNameText.transform.parent.gameObject.SetActive(true);
		unitLevelText.text = $"LVL: {unit.level}";
        unitLevelText.transform.parent.gameObject.SetActive(true);
        isOccupied = true;

		// Uncomment to show the remove unit sign
        // removeSign.SetActive(isPlayer);
		
        GetComponent<Button>().interactable = isPlayer;

        unitImage.sprite = selectedUnit.character.inGameSprite;
        unitImage.gameObject.SetActive(true);
    }

    public void UnselectUnit() {
        unitNameText.transform.parent.gameObject.SetActive(false);
		unitNameText.text = String.Empty;
        unitLevelText.transform.parent.gameObject.SetActive(false);
		unitLevelText.text = String.Empty;
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
