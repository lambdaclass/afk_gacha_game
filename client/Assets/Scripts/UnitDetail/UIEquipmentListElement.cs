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
    Image itemIcon;

    [SerializeField]
    Button equipButton;

    [SerializeField]
    Button unequipButton;

    [SerializeField]
    TMP_Text itemLevelUpText;

    public void SetItemInfo(UnitDetail unitDetail, Item item) {
        this.unitDetail = unitDetail;
        this.item = item;
        itemNameText.text = item.template.name;
        itemLevelText.text = $"Level: {item.level}";
        // Currently no way to know which resource is needed to level up the weapon
        itemLevelUpText.text = $"Level Up ({item.GetLevelUpCost()})";
        // We don't currently get the image from the backend but it should be set up here.

        if(!String.IsNullOrEmpty(this.item.unitId)) {
            equipButton.gameObject.SetActive(false);
            unequipButton.gameObject.SetActive(true);
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
    }

    public void LevelUpItem() {
        SocketConnection.Instance.LevelUpItem(GlobalUserData.Instance.User.id, this.item.id, (item) => {
            GlobalUserData.Instance.User.items.Find(item => item.id == this.item.id).level = item.level;
            this.item.level = item.level;
            itemLevelText.text = $"Level: {item.level}";
            // Currently no way to know which resource is needed to level up the weapon
            itemLevelUpText.text = $"Level Up ({item.GetLevelUpCost()})";
        });
    }
}
