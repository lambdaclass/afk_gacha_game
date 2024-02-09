using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemEquipSlot : MonoBehaviour
{
    [SerializeField]
    GameObject ItemListPopup;

    public void OpenItemListPopup() {
        ItemListPopup.SetActive(true);
    }
}
