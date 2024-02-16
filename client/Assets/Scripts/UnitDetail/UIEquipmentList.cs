using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIEquipmentList : MonoBehaviour
{
    [SerializeField]
    GameObject UIItemPrefab;

    [SerializeField]
    GameObject UIItemsContainer;

    [SerializeField]
    UnitDetail unitDetail;

    void OnEnable()
    {
        foreach (Transform child in UIItemsContainer.transform) {
            Destroy(child.gameObject);
        }

        foreach(Item item in GlobalUserData.Instance.User.items.Where(item => item.template.type == UIEquipmentSlot.selctedEquipmentSlot.EquipmentType).OrderBy(item => String.IsNullOrEmpty(item.unitId))) {
            GameObject ItemGO = Instantiate(UIItemPrefab, UIItemsContainer.transform);
            UIEquipmentListElement ItemUI = ItemGO.GetComponent<UIEquipmentListElement>();
            ItemUI.SetItemInfo(unitDetail, item);
        }
    }
}
