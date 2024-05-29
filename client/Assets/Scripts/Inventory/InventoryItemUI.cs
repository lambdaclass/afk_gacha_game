using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemUI : MonoBehaviour
{
    [SerializeField]
    Image icon;
    [SerializeField]
    TMP_Text quantityText;
    public void SetUpItem(Item item, int quantity)
    {
        icon.sprite = item.template.icon;

        if (quantity > 1)
        {
            quantityText.gameObject.SetActive(true);
            quantityText.text = quantity.ToString();
        }
    }
}
