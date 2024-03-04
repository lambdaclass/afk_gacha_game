using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentListElement : MonoBehaviour
{
    UnitDetail unitDetail;

    Item item;

    [SerializeField]
    TMP_Text itemNameText;

    [SerializeField]
    TMP_Text itemLevelText;
    
    [SerializeField]
    Image itemIconUI;

    [SerializeField]
    Button equipButton;

    [SerializeField]
    Button unequipButton;

    [SerializeField]
    TMP_Text itemLevelUpText;

    [SerializeField]
    UnitItemUI currentUnitIconUI;

    public void SetItemInfo(UnitDetail unitDetail, Item item, ChangeItemUnitPopup changeItemUnitPopup) {
        this.unitDetail = unitDetail;
        this.item = item;
        itemNameText.text = item.template.name;
        itemIconUI.sprite = item.template.icon;
        itemLevelText.text = $"Level: {item.level}";
        // Currently no way to know which resource is needed to level up the weapon.
        itemLevelUpText.text = $"Level Up ({item.GetLevelUpCost()})";
        // We don't currently get the image from the backend but it should be set up here.

        if(this.item.unitId == UnitDetail.GetSelectedUnit().id) {
            currentUnitIconUI.gameObject.SetActive(false);
            equipButton.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(true);
        } else if(!String.IsNullOrEmpty(this.item.unitId)) {
            currentUnitIconUI.gameObject.SetActive(true);
            // Not the prettiest code
            Unit currentUnit = GlobalUserData.Instance.User.units.Find(unit => unit.id == this.item.unitId);
            currentUnitIconUI.SetUpUnitItemUI(currentUnit);
            equipButton.GetComponentInChildren<TMP_Text>().text = "Change";
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(() => {
                changeItemUnitPopup.SetData(item, currentUnit, UnitDetail.GetSelectedUnit(), EquipItem);
            });
        } else {
            currentUnitIconUI.gameObject.SetActive(false);
            equipButton.GetComponentInChildren<TMP_Text>().text = "Equip";
            equipButton.onClick.RemoveAllListeners();
            equipButton.onClick.AddListener(EquipItem);
        }
    }

    public void EquipItem() {
        unitDetail.EquipItem(item.id, UnitDetail.GetSelectedUnit().id);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(true);
    }

    public void UnequipItem() {
        unitDetail.UnequipItem(item.id);
        equipButton.gameObject.SetActive(true);
        unequipButton.gameObject.SetActive(false);
        currentUnitIconUI.gameObject.SetActive(false);
        equipButton.GetComponentInChildren<TMP_Text>().text = "Equip";
        equipButton.onClick.RemoveAllListeners();
        equipButton.onClick.AddListener(EquipItem);
    }

    public void LevelUpItem() {
        unitDetail.LevelUpItem(this.item, (item) => {
            // Check if level of item returned changed, if not then the level up wasn't successful (should refactor)
            if(item.level > this.item.level) {
                GlobalUserData.Instance.User.AddIndividualCurrency(Currency.Gold, -(int)Math.Round(Math.Pow(this.item.level, 2)));
                GlobalUserData.Instance.User.items.Find(item => item.id == this.item.id).level = item.level;
                this.item.level = item.level;
                itemLevelText.text = $"Level: {item.level}";
                // Currently no way to know which resource is needed to level up the weapon
                itemLevelUpText.text = $"Level Up ({item.GetLevelUpCost()})";
            }
        });
    }
}
