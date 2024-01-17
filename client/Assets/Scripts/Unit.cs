using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public string id { get; set; }
    public Rank rank { get; set; }
    public int tier { get; set; }
    public int level { get; set; }
    public Character character { get; set; }
    public int? slot { get; set; }
    public bool selected { get; set; }

    public Item head = null;

    public Item chest = null;

    public Item weapon = null;

    public Item boots = null;

    ///////////
    // Items //
    ///////////

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
        if (item.equippedTo != null) { item.equippedTo.UnequipItem(item.type); }
        
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

        item.equippedTo = this;
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

    //////////
    // Rank //
    //////////

    public bool RankUp() { 
        if(CanRankUp()){
            rank = rank.Next();
            return true;
        }
        return false;
    }

    public bool CanRankUp() {
        // Common champions can't upgrade Rank.
        // Rare champions can only get to 5 Stars.
        return character.rarity != Rarity.Common && rank < GetMaxRank(character.rarity);
    }

    public static Rank GetMaxRank(Rarity rarity) {
        switch (rarity) {
            case Rarity.Common:
                return Rank.Star3;
            case Rarity.Rare:
                return Rank.Star5;
            case Rarity.Elite:
                return Rank.Awakened;
            default:
                return Rank.Awakened;
        } 
    }

    //////////
    // Tier //
    //////////

    public bool TierUp() {
        if (CanTierUp()) {
            tier++; 
            return true;
        }
        return false;
    }

    public bool CanTierUp() {
        return tier < GetMaxTier(rank);
    }

    public Dictionary<Currency, int> TierUpCost {
        get { return new Dictionary<Currency, int>(){{Currency.Gold, (int)Math.Floor(Math.Pow(level, 1 + level / 30.0))}, {Currency.Gems, 50}}; }
    }

    public static int GetMaxTier(Rank rank) {
        switch (rank) {
            case Rank.Star1:
                return 1;
            case Rank.Star2:
                return 2;
            case Rank.Star3:
                return 3; 
            case Rank.Star4:
                return 4;
            case Rank.Star5:
                return 5; 
            case Rank.Ilumination1:
                return 7; 
            case Rank.Ilumination2:
                return 9; 
            case Rank.Ilumination3:
                return 220; 
            case Rank.Awakened:
                return 12; 
            default:
                return 0;
        }
    }

    ///////////
    // Level //
    ///////////

    public bool LevelUp() { 
        if (CanLevelUp()) {
            level++; 
            return true;
        }
        return false;
    }

    public bool CanLevelUp() {
        return level < GetMaxLevel(tier);
    }

    public Dictionary<Currency, int> LevelUpCost {
        get { return new Dictionary<Currency, int>(){{Currency.Gold, (int)Math.Floor(Math.Pow(level, 1 + level / 30.0))}}; }
    }

    public static int GetMaxLevel(int tier) {
        switch (tier) {
            case 0:
                return 10;
            case 1:
                return 20;
            case 2:
                return 40;
            case 3:
                return 60; 
            case 4:
                return 80;
            case 5:
                return 100; 
            case 6:
                return 120; 
            case 7:
                return 140; 
            case 8:
                return 160; 
            case 9:
                return 180; 
            case 10:
                return 200; 
            case 11:
                return 220; 
            case 12:
                return 250; 
            default:
                return 0;
        }
    }
}

public enum Rank{
    Star1,
    Star2,
    Star3,
    Star4,
    Star5,
    Ilumination1,
    Ilumination2,
    Ilumination3,
    Awakened
}
