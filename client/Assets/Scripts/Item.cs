using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// "ItemInstance" rethink names? In backend is like this
public class Item
{
    public string id;
    public int level;
    public ItemTemplate template;
    public string userId;
    public string unitId;
    // Hardcoded sprite, should get from backend
    public Sprite icon = Resources.Load<Sprite>("UI/Equipment/PlaceholderSprite");

    public int GetLevelUpCost() {
        return (int)Math.Pow(level, 2);
    }
}

public class ItemTemplate
{
    public string id;
    public string name;
    public string type;
}
