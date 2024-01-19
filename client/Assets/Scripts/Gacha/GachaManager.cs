using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class GachaManager : MonoBehaviour
{
    [SerializeField]
    GameObject modelContainer;

    [SerializeField]
    GameObject characterNameContainer;

    [SerializeField]
    GameObject characterContainerButton;

    [SerializeField]
    List<ConcreteItem> concreteItems;

    private Unit currentUnit;
    public void RollCharacter(Box box)
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;
        User user = globalUserData.User;

        Character rolledCharacter = box.RollChampion();

        // Add the rolled character to the user's units.
        Unit unit = AddNewUnit(user, rolledCharacter);
        DisplayCharacter(rolledCharacter);

        // For testing unequip, we first check if we had already created the sword. If we did, we set a new owner instead.
        Item sword;
        if (user.items.Count > 0) {
            sword = user.items[0];
        } else {
            // For testing the equipables, we'll create a sword when we summon
            sword = new Item(
                "Epic Sword of Epicness",
                new List<Effect>(){
                    new Effect{attribute = Attribute.Level, modifier = new AdditiveModifier{value = 100}}
                },
                EquipType.Weapon,
                concreteItems.Find(item => item.name == "Sword")
            );

            user.AddItem(sword);
        }
        unit.EquipItem(sword);
    }

    private Unit AddNewUnit(User user, Character character)
    {
        Unit newUnit = new Unit
        {
            id = user.NextId(),
            level = 1,
            character = character,
            selected = false
        };

        user.units.Add(newUnit);
        currentUnit = newUnit;
        characterContainerButton.GetComponent<ButtonAnimations>().clickEvent.AddListener(() => SelectUnit());
        return newUnit;
    }

    private void DisplayCharacter(Character character)
    {
        if (modelContainer.transform.childCount > 0)
        {
            Destroy(modelContainer.transform.GetChild(0).gameObject);
        }
        Instantiate(character.prefab, modelContainer.transform);
        characterNameContainer.GetComponentInChildren<TextMeshProUGUI>().text = character.name;
        characterNameContainer.SetActive(true);
    }

    public void SelectUnit()
    {
        if (currentUnit == null)
        {
            return;
        }
        UnitDetail.SelectUnit(currentUnit);
    }
}
