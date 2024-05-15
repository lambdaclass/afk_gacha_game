using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeItemUnitPopup : MonoBehaviour
{
    [SerializeField]
    Image itemImage;

    [SerializeField]
    UnitItemUI currentUnitIconUI;

    [SerializeField]
    UnitItemUI targetUnitIconUI;

    [SerializeField]
    Button confirmButtonUI;

    public void SetData(Item item, Unit currentUnit, Unit targetUnit, Action equipItem)
    {
        itemImage.sprite = item.template.icon;
        currentUnitIconUI.SetUpUnitItemUI(currentUnit);
        targetUnitIconUI.SetUpUnitItemUI(targetUnit);
        confirmButtonUI.onClick.RemoveAllListeners();
        confirmButtonUI.onClick.AddListener(() =>
        {
            equipItem();
            this.gameObject.SetActive(false);
        });
        gameObject.SetActive(true);
    }
}
