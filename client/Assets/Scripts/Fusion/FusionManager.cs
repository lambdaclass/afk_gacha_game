using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FusionManager : MonoBehaviour, IUnitPopulator
{
    [SerializeField] GameObject modelContainer;

    [SerializeField] Button fusionButton;

    [SerializeField] UnitsUIContainer unitsContainer;

    [SerializeField] List<Character> characters;

    private List<Unit> selectedUnits;

    GlobalUserData globalUserData = GlobalUserData.Instance;

    void Start()
    {
        List<Unit> units = globalUserData.Units;
        selectedUnits = new List<Unit>();
        fusionButton.onClick.AddListener(Fusion);
        this.unitsContainer.Populate(units, this);
    }

    public void Populate(Unit unit, GameObject unitUIItem)
    {
        // SpriteState ss = new SpriteState();
        // ss.highlightedSprite = unit.character.iconSprite;
        // Button unitItemButton = unitItem.GetComponent<Button>();
        // unitItemButton.spriteState = ss;
        // if(unit.selected) {
        //     unitItemButton.interactable = false;
        // }
        unitUIItem.GetComponent<Image>().sprite = unit.character.iconSprite;
        Button unitItemButton = unitUIItem.GetComponent<Button>();
        unitItemButton.onClick.AddListener(() => SelectUnit(unit));
    }

    public void SelectUnit(Unit unit)
    {
        if (modelContainer.transform.childCount > 0)
        {
            Destroy(modelContainer.transform.GetChild(0).gameObject);
        }
        Instantiate(unit.character.prefab, modelContainer.transform);
        unit.selected = true;
        selectedUnits.Add(unit);
        fusionButton.gameObject.SetActive(true);
    }

    public void UnselectUnit(Unit unit)
    {
        unit.selected = false;
        selectedUnits.Remove(unit);
        if (selectedUnits.Count == 0)
        {
            fusionButton.gameObject.SetActive(false);
        }
    }

    public void Fusion() {
        globalUserData.User.FuseUnits(selectedUnits);
    }
}
