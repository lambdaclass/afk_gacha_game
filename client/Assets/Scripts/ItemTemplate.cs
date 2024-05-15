using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Template", menuName = "Item Template")]
public class ItemTemplate : ScriptableObject
{
    public string name;
    public string type;
    public Sprite icon;
}
