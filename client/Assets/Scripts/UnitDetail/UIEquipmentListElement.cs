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

    public void SetItemInfo(UnitDetail unitDetail, Item item) {
        this.unitDetail = unitDetail;
        this.item = item;
        itemNameText.text = item.template.name;
        itemLevelText.text = $"Level: {item.level}";
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
}
