using System.Collections;
using System.Collections.Generic;

// "ItemInstance" rethink names? In backend is like this
public class Item
{
    public string id;
    public int level;
    public ItemTemplate template;
    public string userId;
    public string unitId;  
}

public class ItemTemplate
{
    public string id;
    public string name;
    public string type;
}
