using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDetailPopup : MonoBehaviour
{
    [SerializeField]
    Image icon;
    [SerializeField]
    new TMP_Text name;
    [SerializeField]
    TMP_Text rarity;
    [SerializeField]
    TMP_Text level;
    [SerializeField]
    TMP_Text type;
    [SerializeField]
    TMP_Text attributes;
    public void ShowItem(Item item)
    {
        icon.sprite = item.template.icon;
        name.text = item.template.name;
        rarity.text = "Common"; // Hardcoded, don't currently get attributes from the backend.
        level.text = $"Level: {item.level}";
        type.text = $"Type: {item.template.type}";
        attributes.text = "+50hp"; // Hardcoded, don't currently get attributes from the backend.
        gameObject.SetActive(true);
    }
}