using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineupManager : MonoBehaviour
{
    [SerializeField]
    CharacterList characterList;

    void Start()
    {
        StartCoroutine(
            BackendConnection.GetAvailableUnits(
                units => {
                    characterList.PopulateList(units);
                }
            )
        );
    }
}
