using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CharacterList : MonoBehaviour
{
    [SerializeField]
    GameObject CharacterItemUIPrefab;

    [SerializeField]
    GameObject CharacterListContainer;

    [NonSerialized]
    public UnityEvent<Unit> OnCharacterSelected = new UnityEvent<Unit>();

    public void PopulateList(List<Unit> units)
    {
        units.ForEach(unit =>
        {
            print($"{unit.character.name}, {unit.unit_id}");
            GameObject characterItem = Instantiate(CharacterItemUIPrefab, CharacterListContainer.transform);
            characterItem.GetComponent<Image>().sprite = unit.character.characterSprite;
            characterItem.GetComponent<Button>().onClick.AddListener(() => SelectCharacter(unit));
        });
    }

    public void SelectCharacter(Unit unit)
    {
        OnCharacterSelected.Invoke(unit);
    }
}
