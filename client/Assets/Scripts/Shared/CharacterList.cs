using System;
using UnityEngine;
using UnityEngine.Events;

public class CharacterList : MonoBehaviour
{
    [SerializeField]
    GameObject CharacterItemUIPrefab;

    [NonSerialized]
    public UnityEvent<string> OnCharacterSelected;

    void Start() {
        OnCharacterSelected = new UnityEvent<string>();
    }

    public void SelectCharacter(string character)
    {
        OnCharacterSelected.Invoke(character);
    }
}
