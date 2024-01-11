using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public string id { get; set; }
    public string username { get; set; }

    public List<Unit> units { get; set; }

    public int next_unit_id;

    public Dictionary<string, int> currencies = new Dictionary<string, int>();

    public string NextId(){
        string next_id = next_unit_id.ToString();
        next_unit_id = next_unit_id + 1;
        return next_id;
    }
}
