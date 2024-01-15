using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FusionManager : MonoBehaviour, IUnitPopulator
{
    [SerializeField]
    GameObject modelContainer;

    [SerializeField]
    UnitsUIContainer unitsContainer;

    [SerializeField]
    List<Character> characters;

    void Start()
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;
        OpponentData opponentData = OpponentData.Instance;

        List<Unit> units = globalUserData.Units;

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
    }
}
