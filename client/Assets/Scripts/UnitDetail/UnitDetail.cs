using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UnitDetail : MonoBehaviour
{
    private static Unit selectedUnit;

    [SerializeField]
    TMP_Text goldCostText;

    [SerializeField]
    TMP_Text unitName;

    [SerializeField]
    Image backgroundImage;

    void Start() {
        unitName.text = $"{selectedUnit.character.name}, lvl: {selectedUnit.level}";
        goldCostText.text = selectedUnit.LevelUpCost["gold"].ToString();
        backgroundImage.sprite = selectedUnit.character.backgroundSprite;
    }

    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
        SceneManager.LoadScene("UnitDetail");
    }

    public void LevelUp() {
        if (CanLevelUp()) {
            selectedUnit.LevelUp();
            unitName.text = $"{selectedUnit.character.name}, lvl: {selectedUnit.level}";
            goldCostText.text = selectedUnit.LevelUpCost["gold"].ToString();
        }
    }

    public bool CanLevelUp() {
        // Waiting on: #22
        // if CanUserBuyItem(globalUserData.User, selectedUnit.LevelUpCost) {}
        return true;
    }
}
