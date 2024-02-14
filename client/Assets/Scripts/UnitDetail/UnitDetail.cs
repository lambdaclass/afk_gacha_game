using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class UnitDetail : MonoBehaviour
{
    private static Unit selectedUnit;

    [SerializeField]
    TMP_Text goldCostText;

    [SerializeField]
    TMP_Text gemCostText;
    
    [SerializeField]
    Text actionButtonText;

    [SerializeField]
    TMP_Text unitName;

    [SerializeField]
    Image backgroundImage;

    private Dictionary<Currency, int> cost;

    [SerializeField]
    List<UIEquipmentSlot> equipmentSlots;

    // true if we're leveling up, false if we're tiering up
    private bool actionLevelUp;

    void Start() {
        SetActionAndCosts();
        UpdateTexts();
        backgroundImage.sprite = selectedUnit.character.selectedSprite;
    }

    public void ActionButton() {
        User user = GlobalUserData.Instance.User;
        if (user.CanAfford(cost)) {
            bool result;
            if (actionLevelUp) { result = LevelUp(); } else { result = TierUp(); }

            if (result) { 
                user.SubstractCurrency(cost);
                SetActionAndCosts();
                UpdateTexts();
            }
        } else {
            // Blocked by currency cost.
        }
    }

    private bool LevelUp() {
        if (selectedUnit.LevelUp()) {
            return true;
        }
        Debug.LogError("[UnitDetail.cs] Could not level up unit. Likely cause: a disparity between User.CanLevelUp() calls in UnitDetail.SetActionAndCosts() and Unit.LevelUp().");
        return false;
    }

    private bool TierUp() {
        if (selectedUnit.TierUp()) {
            return true;
        }
        // Tell user they need to improve the ranking of their unit via fusion.
        return false;
    }

    // I think both SelectUnit and GetSelectedUnit should be removed and the selectedUnit field be made public
    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
        UnityEngine.SceneManagement.SceneManager.LoadScene("UnitDetail");
    }

    public static Unit GetSelectedUnit() {
        return selectedUnit;
    }

    private void UpdateTexts() {
        if (actionLevelUp) actionButtonText.text = "Level Up";
        else actionButtonText.text = "Tier up";

        unitName.text = $"{selectedUnit.character.name}, tier/lvl: {selectedUnit.tier}/{selectedUnit.level} {selectedUnit.rank.ToString()}";
        goldCostText.text = cost.ContainsKey(Currency.Gold) ? cost[Currency.Gold].ToString() : "0";
        gemCostText.text = cost.ContainsKey(Currency.Gems) ? cost[Currency.Gems].ToString() : "0";
    }

    private void SetActionAndCosts() {
        if (selectedUnit.CanLevelUp()) {
            actionLevelUp = true;
            cost = selectedUnit.LevelUpCost;
        } else {
            actionLevelUp = false;
            cost = selectedUnit.TierUpCost;
        }
    }

    public void EquipItem(string itemId, string unitId)
    {
        SocketConnection.Instance.EquipItem(GlobalUserData.Instance.User.id, itemId, unitId, (item) => {
            UIEquipmentSlot.selctedEquipmentSlot.SetEquippedItem(item);
            GlobalUserData.Instance.User.items.Find(item => item.id == itemId).unitId = unitId;
        });
    }

    public void UnequipItem(string itemId)
    {
        SocketConnection.Instance.UnequipItem(GlobalUserData.Instance.User.id, itemId, (item) => {
            UIEquipmentSlot.selctedEquipmentSlot.SetEquippedItem(null);
            GlobalUserData.Instance.User.items.Find(item => item.id == itemId).unitId = null;
        });
    }
}
