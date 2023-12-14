using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerLineupManager : MonoBehaviour
{
    [SerializeField]
    CharacterList characterList;

    [SerializeField]
    List<UnitPosition> unitPositions;

    void Start() {
        characterList.OnCharacterSelected.AddListener(AddCharacterToLineup);
    }

    private void AddCharacterToLineup(string character)
    {
        print($"selected character: {character}");
        UnitPosition up = unitPositions.FirstOrDefault(unitPosition => !unitPosition.IsOccupied);
        if(up)
        {
            up.SetCharacter(character);
        }
    }
}
