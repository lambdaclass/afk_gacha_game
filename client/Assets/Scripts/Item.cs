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
    public int GetLevelUpCost()
    {
        return (int)Math.Pow(level, 2);
    }
}