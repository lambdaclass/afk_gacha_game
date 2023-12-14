using UnityEngine;
using TMPro;
using System;

public class UnitPosition : MonoBehaviour
{
    [SerializeField]
    TMP_Text unitName;

    private bool isOccupied;
    public bool IsOccupied => isOccupied;

    public void SetCharacter(Character character) {
        unitName.text = character.name;
        isOccupied = true;
        unitName.gameObject.SetActive(true);
        GameObject newUnit = Instantiate(character.prefab, this.transform);
        Vector3 newUnitPosition = newUnit.transform.position;
        newUnitPosition.z = -100;
        newUnit.transform.position = newUnitPosition;
    }
}
