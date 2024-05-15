using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    GameObject itemPrefab;
    [SerializeField]
    GameObject itemsContainer;
    [SerializeField]
    ItemDetailPopup itemDetailPopup;

    void Start()
    {
        foreach (var group in GlobalUserData.Instance.User.items.GroupBy(item => item.template.name))
        {
            GameObject itemUIObject = Instantiate(itemPrefab, itemsContainer.transform);
            itemUIObject.GetComponent<InventoryItemUI>().SetUpItem(group.First(), group.Count());
            Button unitItemButton = itemUIObject.GetComponent<Button>();
            unitItemButton.onClick.AddListener(() => ShowItemDetailPopup(group.First()));
        }
    }

    private void ShowItemDetailPopup(Item item)
    {
        itemDetailPopup.ShowItem(item);
    }
}
