using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnitDetail : MonoBehaviour
{
    private static Unit selectedUnit;

    [SerializeField]
    TMP_Text unitName; 

    void Start() {
        unitName.text = selectedUnit.name;
    }

    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
        SceneManager.LoadScene("UnitDetail");
    }
}
