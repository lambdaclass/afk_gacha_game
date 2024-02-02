using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FusionManager : MonoBehaviour, IUnitPopulator
{
    [SerializeField] GameObject modelContainer;
    [SerializeField] Button fusionButton;
    [SerializeField] FusionUnitsUIContainer unitsContainer;
    [SerializeField] GameObject characterNameContainer;

    private List<Unit> selectedUnits;

    GlobalUserData globalUserData = GlobalUserData.Instance;

    void Start()
    {
        List<Unit> units = globalUserData.Units;
        selectedUnits = new List<Unit>();
        this.unitsContainer.Populate(units, this);
    }

    public void Populate(Unit unit, GameObject unitUIItem)
    {
        unitUIItem.GetComponentInChildren<Image>().sprite = unit.character.defaultSprite;
        Button unitItemButton = unitUIItem.GetComponent<Button>();
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
            RemoveUnitFromContainer(unit);
            fusionButton.gameObject.SetActive(false);
        }
    }

    private void DisplayUnit(Unit unit)
    {
        if (modelContainer.transform.childCount > 0)
        {
            RemoveUnitFromContainer(unit);
        }
        Instantiate(unit.character.prefab, modelContainer.transform);
        characterNameContainer.GetComponentInChildren<TextMeshProUGUI>().text = unit.character.name + "\n Rank: " + unit.rank.ToString();
        characterNameContainer.SetActive(true);
    }

    private void RemoveUnitFromContainer(Unit unit)
    {
        Destroy(modelContainer.transform.GetChild(0).gameObject);
        characterNameContainer.SetActive(false);
    }

    public void Fusion() {
        globalUserData.User.FuseUnits(selectedUnits);
        this.unitsContainer.Populate(globalUserData.Units, this);
        selectedUnits.Clear();
        fusionButton.gameObject.SetActive(false);
    }
}
