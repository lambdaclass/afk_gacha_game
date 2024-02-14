using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEquipmentSlot : MonoBehaviour
{
    [SerializeField]
    string equipmentType;
    public string EquipmentType => equipmentType;

    [SerializeField]
    GameObject ItemListPopup;

    [SerializeField]
    Image slotIcon;

    [SerializeField]
    Sprite addIconSprite;

    [SerializeField]
    Sprite removeIconSprite;

    [SerializeField]
    Image actionIcon;

    Item equippedItem;

    public static UIEquipmentSlot selctedEquipmentSlot;

    public void OpenItemListPopup() {
        selctedEquipmentSlot = this;
        ItemListPopup.SetActive(true);
    }

    public void SetEquippedItem(Item item)
    {
        this.equippedItem = item;
        if(item != null) {
            // Currently we don't get items images from the backend but it should be assigned here

            actionIcon.sprite = removeIconSprite;
        }
        else {
            actionIcon.sprite = addIconSprite;
        }
    }
}
