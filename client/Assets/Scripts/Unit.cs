using System;
using System.Collections.Generic;

public class Unit
{
    public string id { get; set; }
    public Quality quality { get; set; }
    public int tier { get; set; }
    public int level { get; set; }
    public Character character { get; set; }
    public int? slot { get; set; }
    public bool selected { get; set; }

    /////////////
    // Quality //
    /////////////

    public bool QualityUp() { 
        if(CanQualityUp()){
            quality = quality.Next();
            return true;
        }
        return false;
    }

    public bool CanQualityUp() {
        // Common champions can't upgrade Quality.
        // Rare champions can only get to 5 Stars.
        return character.rarity != Rarity.Common && quality < GetMaxQuality(character.rarity);
    }

    public static Quality GetMaxQuality(Rarity rarity) {
        switch (rarity) {
            case Rarity.Common:
                return Quality.Star3;
            case Rarity.Rare:
                return Quality.Star5;
            case Rarity.Elite:
                return Quality.Awakened;
            default:
                return Quality.Awakened;
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
        return tier < GetMaxTier(quality);
    }

    public Dictionary<string, int> TierUpCost {
        get { return new Dictionary<string, int>(){{"gold", (int)Math.Floor(Math.Pow(level, 1 + level / 30.0))}, {"gems", 50}}; }
    }

    public static int GetMaxTier(Quality quality) {
        switch (quality) {
            case Quality.Star1:
                return 1;
            case Quality.Star2:
                return 2;
            case Quality.Star3:
                return 3; 
            case Quality.Star4:
                return 4;
            case Quality.Star5:
                return 5; 
            case Quality.Ilumination1:
                return 7; 
            case Quality.Ilumination2:
                return 9; 
            case Quality.Ilumination3:
                return 220; 
            case Quality.Awakened:
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

    public Dictionary<string, int> LevelUpCost {
        get { return new Dictionary<string, int>(){{"gold", (int)Math.Floor(Math.Pow(level, 1 + level / 30.0))}}; }
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

public enum Quality{
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
