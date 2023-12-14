using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterList : MonoBehaviour
{
    [SerializeField]
    GameObject CharacterItemUIPrefab;

    [NonSerialized]
    public UnityEvent<Character> OnCharacterSelected;

    void Start() {
        OnCharacterSelected = new UnityEvent<Character>();
    }

    public void SelectCharacter(Character character)
    {
        OnCharacterSelected.Invoke(character);
    }
}
