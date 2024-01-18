using UnityEngine;

[CreateAssetMenu(fileName = "New Concrete Item", menuName = "Concrete Items")]
public class ConcreteItem : ScriptableObject
{
    public string Name;
    public EquipType Type;
    public Sprite Sprite;

}

