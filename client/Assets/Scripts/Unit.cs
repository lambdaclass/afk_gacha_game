using System;
using System.Collections.Generic;

public class Unit
{
    public string id { get; set; }
    public int level { get; set; }
    public Character character { get; set; }
    public int? slot { get; set; }
    public bool selected { get; set; }

    public Dictionary<string, int> LevelUpCost {
        get { return new Dictionary<string, int>(){{"gold", (int)Math.Floor(Math.Pow(level, 1 + level / 30.0))}}; }
    }

    public void LevelUp() { level++; }
}
