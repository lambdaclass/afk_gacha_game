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
    Image equipmentIcon;

    [SerializeField]
    Sprite addIconSprite;

    [SerializeField]
    Sprite removeIconSprite;

    [SerializeField]
    Image actionIcon;

    Item equippedItem;

    public static UIEquipmentSlot selctedEquipmentSlot;

    public void OpenItemListPopup()
    {
        selctedEquipmentSlot = this;
        ItemListPopup.SetActive(true);
    }

    public void SetEquippedItem(Item item)
    {
        this.equippedItem = item;
        if (item != null)
        {
            equipmentIcon.sprite = item.template.icon;
            equipmentIcon.gameObject.SetActive(true);
            slotIcon.gameObject.SetActive(false);
            actionIcon.sprite = removeIconSprite;
        }
        else
        {
            equipmentIcon.sprite = null;
            equipmentIcon.gameObject.SetActive(false);
            slotIcon.gameObject.SetActive(true);
            actionIcon.sprite = addIconSprite;
        }
    }
}
