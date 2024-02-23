using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AscensionManager : MonoBehaviour, IUnitPopulator
{
    [SerializeField]
    Button fusionButton;
    [SerializeField]
    FusionUnitsUIContainer unitsContainer;
    [SerializeField]
    GameObject characterNameContainer;
    [SerializeField]
    Image selectedCharacterImage;

    private List<Unit> selectedUnits;

    User user = GlobalUserData.Instance.User;

    void Start()
    {
        List<Unit> units = user.units;
        selectedUnits = new List<Unit>();
        this.unitsContainer.Populate(units, this);
    }

    public void Populate(Unit unit, GameObject unitItem)
    {
        unitItem.GetComponent<UnitItemUI>().SetUpUnitItemUI(unit);
        Button unitItemButton = unitItem.GetComponent<Button>();
        unitItemButton.onClick.AddListener(() => {
            if (selectedUnits.Contains(unit))
            {
                UnselectUnit(unit);
            }
            else
            {
                SelectUnit(unit);
            }
        });
    }

    public void SelectUnit(Unit unit)
    {
        DisplayUnit(unit);
        selectedUnits.Add(unit);
        unitsContainer.SetUnitsLock(unit.character.faction, true);
        fusionButton.gameObject.SetActive(true);
    }

    public void UnselectUnit(Unit unit)
    {
        selectedUnits.Remove(unit);
        if (selectedUnits.Count > 0)
        {
            DisplayUnit(selectedUnits[selectedUnits.Count - 1]);
        }
        else
        {
            RemoveUnitFromDisplay(unit);
            unitsContainer.SetUnitsLock(unit.character.faction, false);
            fusionButton.gameObject.SetActive(false);
        }
    }

    private void DisplayUnit(Unit unit)
    {
        selectedCharacterImage.sprite = unit.character.inGameSprite;
        characterNameContainer.GetComponentInChildren<TextMeshProUGUI>().text = unit.character.name + "\n Rank: " + unit.rank.ToString();
        selectedCharacterImage.transform.parent.gameObject.SetActive(true);
    }

    private void RemoveUnitFromDisplay(Unit unit)
    {
        selectedCharacterImage.sprite = null;
        characterNameContainer.GetComponentInChildren<TextMeshProUGUI>().text = null;
        selectedCharacterImage.transform.parent.gameObject.SetActive(false);
    }

    public void Fusion() {
        // globalUserData.User.FuseUnits(selectedUnits);
        Debug.LogError("Fusion not yet connected to backend");
        this.unitsContainer.Populate(user.units, this);
        selectedUnits.Clear();
        fusionButton.gameObject.SetActive(false);
    }
}
