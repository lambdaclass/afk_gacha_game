using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public static int nextId = 0;

    public string name;

    public string id { get; }

    public EquipType type;

    public List<Effect> effects = new List<Effect>();

    public Unit equippedTo;

    public Sprite sprite { get; set; }

    public Item(string _name, List<Effect> _effects, EquipType _type) {
        id = NextId();
        name = _name;
        effects = _effects;
        type = _type;
        if (type == EquipType.Head) {
            sprite = Resources.Load<Sprite>("UI/Items/head");
        } else if (type == EquipType.Chest) {
            sprite = Resources.Load<Sprite>("UI/Items/chest");
        } else if (type == EquipType.Weapon) {
            sprite = Resources.Load<Sprite>("UI/Items/weapon");
        } else if (type == EquipType.Boots) {
            sprite = Resources.Load<Sprite>("UI/Items/boots");
        }
    }

    public static string NextId() { return nextId++.ToString(); }
}

public enum EquipType {
    Head,
    Chest,
    Weapon,
    Boots,
    Accessory
}


public class Effect
{
    public Attribute attribute;
    public Modifier modifier;
}

public enum Attribute
{
    // Eventually we'll have actual attributes like health, damage, etc.
    // For now we use the level since that's all we have.
    Level
}

public abstract class Modifier
{
    public abstract int CalculateModification(int baseValue);
}

public class AdditiveModifier : Modifier
{
    public int value;

    public override int CalculateModification(int baseValue)
    {
        return baseValue + value;
    }
}

public class MultiplicativeModifier : Modifier
{
    public float value;

    public override int CalculateModification(int baseValue)
    {
        return (int)(baseValue * value);
    }
}
