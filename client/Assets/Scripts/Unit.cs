using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Unit", menuName = "Units")]
public class Unit : ScriptableObject
{
    public new string name;
    public Sprite unitSprite;
    public GameObject prefab;
}
