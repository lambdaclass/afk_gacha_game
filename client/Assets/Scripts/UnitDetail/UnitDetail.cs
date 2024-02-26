using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;
using System;

public class UnitDetail : MonoBehaviour
{
    private static Unit selectedUnit;

    [SerializeField]
    TMP_Text goldCostText;

    [SerializeField]
    TMP_Text gemCostText;
    
    [SerializeField]
    Text actionButtonText;

    [SerializeField]
    Image backgroundImage;

    [SerializeField]
    GameObject modelContainer;

    [SerializeField]
    GameObject characterNameContainer;

    [SerializeField]
    GameObject levelStatUI;

    [SerializeField]
    GameObject tierStatUI;

    [SerializeField]
    GameObject rankStatUI;

    [SerializeField]
    GameObject headItemSprite;

    [SerializeField]
    GameObject chestItemSprite;

    [SerializeField]
    GameObject bootsItemSprite;

    [SerializeField]
    GameObject weaponItemSprite;

    [SerializeField]
    GameObject insufficientCurrencyPopup;
    [SerializeField]
    GameObject needToTierUpPopup;

    [SerializeField]
    List<UIEquipmentSlot> equipmentSlots;

    // true if we're leveling up, false if we're tiering up
    private bool actionLevelUp;

    void Start() {
        SetUpEquipment();
        SetBackgroundImage();
        DisplayUnit();
    }

    public void LevelUp() {
        SocketConnection.Instance.LevelUpUnit(GlobalUserData.Instance.User.id, selectedUnit.id,
        (unitAndCurrencies) => {
            foreach(var userCurrency in unitAndCurrencies.UserCurrency) {
                GlobalUserData.Instance.User.SetCurrencyAmount((Currency)Enum.Parse(typeof(Currency), userCurrency.Currency.Name), (int)userCurrency.Amount);
            }
            // Should this be encapsulated somewhere?
            GlobalUserData.Instance.User.units.Find(unit => unit.id == unitAndCurrencies.Unit.Id).level++;
        },
        (reason) => {
            switch(reason) {
                case "cant_afford":
                    insufficientCurrencyPopup.SetActive(true);
                    break;
                case "cant_level_up":
                    needToTierUpPopup.SetActive(true);
                    break;
                default:
                    Debug.LogError(reason);
                    break;
            }
        });
    }

    // I think both SelectUnit and GetSelectedUnit should be removed and the selectedUnit field be made public
    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
        SceneManager.LoadScene("UnitDetail");
    }

    public static Unit GetSelectedUnit() {
        return selectedUnit;
    }

    public void EquipItem(string itemId, string unitId)
    {
        SocketConnection.Instance.EquipItem(GlobalUserData.Instance.User.id, itemId, unitId, (item) => {
            UIEquipmentSlot.selctedEquipmentSlot.SetEquippedItem(item);
            // Should this be encapsulated somewhere?
            GlobalUserData.Instance.User.items.Find(item => item.id == itemId).unitId = unitId;
        });
    }

    public void UnequipItem(string itemId)
    {
        SocketConnection.Instance.UnequipItem(GlobalUserData.Instance.User.id, itemId, (item) => {
            UIEquipmentSlot.selctedEquipmentSlot.SetEquippedItem(null);
            // Should this be encapsulated somewhere?
            GlobalUserData.Instance.User.items.Find(item => item.id == itemId).unitId = null;
        });
    }

    public void LevelUpItem(Item item, Action<Item> onItemDataReceived) {
        // Hardcoded to check for gold
        if(item.GetLevelUpCost() > GlobalUserData.Instance.User.GetCurrency(Currency.Gold)) {
            insufficientCurrencyPopup.SetActive(true);
            return;
        }
        SocketConnection.Instance.LevelUpItem(GlobalUserData.Instance.User.id, item.id, (item) => {
            onItemDataReceived?.Invoke(item);
        }, (reason) => {
            if(reason == "cant_afford") {
                insufficientCurrencyPopup.SetActive(true);
            }
        });
    }

    private void SetUpEquipment()
    {
        foreach(Item item in GlobalUserData.Instance.User.items.Where(item => item.unitId == selectedUnit.id)) {
            equipmentSlots.Find(slot => slot.EquipmentType == item.template.type).SetEquippedItem(item);
        }
    }
    private void SetBackgroundImage() 
    {
        switch (selectedUnit.character.faction) 
        {
            case Faction.Araban:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/ArabanBackground");
                break;
            case Faction.Kaline:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/KalineBackground");
                break;
            case Faction.Merliot:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/MerliotBackground");
                break;
            case Faction.Otobi:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/OtobiBackground");
                break;
            default:
                backgroundImage.sprite = Resources.Load<Sprite>("UI/UnitDetailBackgrounds/ArabanBackground");
                break;
        }
    }

    private void DisplayUnit()
    {
        if (modelContainer.transform.childCount > 0)
        {
            RemoveUnitFromContainer();
        }
        Instantiate(selectedUnit.character.prefab, modelContainer.transform);
        characterNameContainer.GetComponentInChildren<TextMeshProUGUI>().text = selectedUnit.character.name;
        characterNameContainer.SetActive(true);
    }

    private void RemoveUnitFromContainer()
    {
        Destroy(modelContainer.transform.GetChild(0).gameObject);
        characterNameContainer.SetActive(false);
    }

    public void PreviousUnit() {
        List<Unit> userUnits = GlobalUserData.Instance.User.units;
        int currentIndex = userUnits.IndexOf(selectedUnit);
        
        int previousIndex = currentIndex - 1;
        if (previousIndex < 0) {
            previousIndex = userUnits.Count - 1;
        }

        Unit previousUnit = userUnits[previousIndex];
        SelectUnit(previousUnit);
    }

    public void NextUnit() {
        List<Unit> userUnits = GlobalUserData.Instance.User.units;
        int currentIndex = userUnits.IndexOf(selectedUnit);
        
        int nextIndex = currentIndex + 1;
        if (nextIndex >= userUnits.Count) {
            nextIndex = 0;
        }

        Unit nextUnit = userUnits[nextIndex];
        SelectUnit(nextUnit);
    }
}
