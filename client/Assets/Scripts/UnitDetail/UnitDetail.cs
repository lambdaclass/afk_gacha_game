using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnitDetail : MonoBehaviour
{
    private static Unit selectedUnit;

    [SerializeField]
    TMP_Text unitName;

    [SerializeField]
    Image background;

    void Start() {
        unitName.text = selectedUnit.name;
        background.sprite = selectedUnit.backgroundSprite;
    }

    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
    }
}
