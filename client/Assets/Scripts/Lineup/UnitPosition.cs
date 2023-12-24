using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;

public class UnitPosition : MonoBehaviour
{
    [SerializeField]
    TMP_Text unitName;

    [SerializeField]
    GameObject modelContainer;

    public event Action<Unit> OnUnitRemoved;

    private bool isOccupied;
    public bool IsOccupied => isOccupied;

    private Unit selectedUnit;

    public string selectedUnitName;

    void Update()
    {
        // Check if spacebar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Print the selectedUnitName
            if (selectedUnit != null)
            {
                Debug.Log("Spacebar pressed. Selected Unit Name: " + selectedUnit.character.name);
            }
            else
            {
                Debug.Log("No unit selected.");
            }
        }
    }


    public void SetUnit(Unit unit) {
        selectedUnit = unit;
        selectedUnitName = selectedUnit.character.name;
        unitName.text = $"{unit.character.name} LVL: {unit.level}";
        isOccupied = true;
        unitName.gameObject.SetActive(true);
        Instantiate(unit.character.prefab, modelContainer.transform);
        GetComponent<Button>().interactable = true;
    }

    public void UnselectUnit() {
        print("a");
        if(selectedUnit != null) {
            print("b");
            print(selectedUnit);
        }
        print(selectedUnitName);
        unitName.text = String.Empty;
        isOccupied = false;
        unitName.gameObject.SetActive(false);
        Destroy(modelContainer.transform.GetChild(0).gameObject);
        GetComponent<Button>().interactable = false;
        OnUnitRemoved?.Invoke(selectedUnit);
        selectedUnit = null;
    }
}
