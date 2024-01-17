using System.Collections.Generic;

public class Unit
{
    public string id { get; set; }
    public int level { get; set; }
    public Character character { get; set; }
    public int? slot { get; set; }
    public bool selected { get; set; }

    public Item head;
    public Item chest;
    public Item weapon;
    public Item boots;

    public List<Item> Items { 
        get {
            List<Item> items = new List<Item>();
            foreach(Item item in new Item[]{head, chest, weapon, boots}) {
                if (item != null) {
                    items.Add(item);
                }
            }
            return items;
        }
    }

    public void EquipItem(Item item) {
        switch (item.type) {
            case EquipType.Head:
                head = item;
                break;
            case EquipType.Chest:
                chest = item;
                break;
            case EquipType.Weapon:
                weapon = item;
                break;
            case EquipType.Boots:
                boots = item;
                break;
            default:
                break;
        }
    }
    public void UnequipItem(EquipType type) {
        switch (type) {
            case EquipType.Head:
                head = null;
                break;
            case EquipType.Chest:
                chest = null;
                break;
            case EquipType.Weapon:
                weapon = null;
                break;
            case EquipType.Boots:
                boots = null;
                break;
            default:
                break;
        }
    }

    public int CalculateLevel() {
        int totalLevel = level;

        if (Items.Count > 0) {
            List<int> levelModifications = new List<int>();
            foreach (Item item in Items) {
                foreach (Effect effect in item.effects) {
                    if (effect.attribute == Attribute.Level) {
                        levelModifications.Add(effect.modifier.CalculateModification(level));
                    }
                }
            }

            if (levelModifications.Count > 0) {
                foreach (int modification in levelModifications) { totalLevel += modification; }
            }
        }

        return totalLevel;
    }
}
