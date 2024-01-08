using System;
using System.Collections.Generic;

[Serializable]
public class User
{
    public string id { get; set; }
    public string username { get; set; }

    public List<Unit> units { get; set; }
}
