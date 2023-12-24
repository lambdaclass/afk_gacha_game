using System;
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
    Image backgroundImage;

    void Start() {
        unitName.text = $"{selectedUnit.character.name}, lvl: {selectedUnit.level}";
        backgroundImage.sprite = selectedUnit.character.backgroundSprite;
    }

    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
        SceneManager.LoadScene("UnitDetail");
    }
}
