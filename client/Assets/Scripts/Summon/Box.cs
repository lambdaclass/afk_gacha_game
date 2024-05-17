using System.Collections.Generic;

public class Box
{
    public string id;
    public string name;
    public string description;
    public List<string> factions;
    public Dictionary<int, int> rankWeights;
    public Dictionary<Currency, int> costs;
}
