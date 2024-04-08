using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class UIEquipmentList : MonoBehaviour
{
    [SerializeField]
    GameObject UIItemPrefab;

    [SerializeField]
    GameObject UIItemsContainer;

    [SerializeField]
    UnitDetail unitDetail;

    [SerializeField]
    ChangeItemUnitPopup changeItemUnitPopup;

	[SerializeField]
	TextMeshProUGUI gold;
	
	[SerializeField]
	TextMeshProUGUI scrolls;

	GlobalUserData user;

    void OnEnable()
    {
		user = GlobalUserData.Instance;
		user.OnCurrencyModified.AddListener(UpdateCurrencyValues);
		UpdateCurrencyValues();

        foreach (Transform child in UIItemsContainer.transform) {
            Destroy(child.gameObject);
        }

        foreach(Item item in GlobalUserData.Instance.User.items.Where(item => item.template.type == UIEquipmentSlot.selctedEquipmentSlot.EquipmentType).OrderBy(item => String.IsNullOrEmpty(item.unitId))) {
            GameObject ItemGO = Instantiate(UIItemPrefab, UIItemsContainer.transform);
            UIEquipmentListElement ItemUI = ItemGO.GetComponent<UIEquipmentListElement>();
            ItemUI.SetItemInfo(unitDetail, item, changeItemUnitPopup);
        }
    }

	void UpdateCurrencyValues()
    {
        gold.text = user.GetCurrency(Currency.Gold).ToString();
		scrolls.text = user.GetCurrency(Currency.SummonScrolls).ToString();
    }
}
