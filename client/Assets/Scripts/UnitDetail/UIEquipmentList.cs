using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipmentList : MonoBehaviour
{
    [SerializeField]
    GameObject UIItemPrefab;

    [SerializeField]
    GameObject UIItemsContainer;

    [SerializeField]
    UnitDetail unitDetail;

    void Start()
    {
        foreach(Item item in GlobalUserData.Instance.User.items) {
            GameObject ItemGO = Instantiate(UIItemPrefab, UIItemsContainer.transform);
            UIEquipmentListElement ItemUI = ItemGO.GetComponent<UIEquipmentListElement>();
            ItemUI.SetItemInfo(unitDetail, item);
        }
    }
}
