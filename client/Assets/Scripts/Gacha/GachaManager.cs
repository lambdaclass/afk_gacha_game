using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class GachaManager : MonoBehaviour
{
    [SerializeField]
    GameObject modelContainer;

    [SerializeField]
    GameObject characterNameContainer;

    [SerializeField]
    GameObject characterContainerButton;

    private Unit currentUnit;
    public void RollCharacter(Box box)
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;
        User user = globalUserData.User;

        Character rolledCharacter = box.RollChampion();

        // Add the rolled character to the user's units.
        AddNewUnit(user, rolledCharacter);
        DisplayCharacter(rolledCharacter);
    }

    private void AddNewUnit(User user, Character character)
    {
        Unit newUnit = new Unit
        {
            id = user.NextId(),
            level = 1,
            character = character,
            selected = false
        };

        user.units.Add(newUnit);
        characterContainerButton.GetComponent<ButtonAnimations>().clickEvent.AddListener(() => SelectUnit(newUnit));
        currentUnit = newUnit;
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
        this.SelectUnit(currentUnit);
    }
    private void SelectUnit(Unit unit) {
        UnitDetail.SelectUnit(unit);
    }
}
