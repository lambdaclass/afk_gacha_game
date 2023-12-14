using UnityEngine;
using TMPro;
using System;

public class UnitPosition : MonoBehaviour
{
    [SerializeField]
    TMP_Text unitName;

    private bool isOccupied;
    public bool IsOccupied => isOccupied;

    public void SetCharacter(string character) {
        unitName.text = character;
        isOccupied = true;
        unitName.gameObject.SetActive(true);
    }
}
