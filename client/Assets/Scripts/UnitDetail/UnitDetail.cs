using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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

    private Dictionary<Currency, int> cost;

    [SerializeField]
    List<UIEquipmentSlot> equipmentSlots;

    // true if we're leveling up, false if we're tiering up
    private bool actionLevelUp;

    void Start() {
        SetUpEquipment();
        SetBackgroundImage();
        DisplayUnit();
    }

    public void ActionButton() {
        Debug.LogError("LevelUp not yet done in backend");
    }

    // I think both SelectUnit and GetSelectedUnit should be removed and the selectedUnit field be made public
    public static void SelectUnit(Unit unit) {
        selectedUnit = unit;
        UnityEngine.SceneManagement.SceneManager.LoadScene("UnitDetail");
    }

    public static Unit GetSelectedUnit() {
        return selectedUnit;
    }

    public void EquipItem(string itemId, string unitId)
    {
        SocketConnection.Instance.EquipItem(GlobalUserData.Instance.User.id, itemId, unitId, (item) => {
            UIEquipmentSlot.selctedEquipmentSlot.SetEquippedItem(item);
            GlobalUserData.Instance.User.items.Find(item => item.id == itemId).unitId = unitId;
        });
    }

    public void UnequipItem(string itemId)
    {
        SocketConnection.Instance.UnequipItem(GlobalUserData.Instance.User.id, itemId, (item) => {
            UIEquipmentSlot.selctedEquipmentSlot.SetEquippedItem(null);
            GlobalUserData.Instance.User.items.Find(item => item.id == itemId).unitId = null;
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
}
