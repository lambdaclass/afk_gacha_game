using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit
{
    public string id { get; set; }
    public int level { get; set; }
    public Character character { get; set; }
    public int? slot { get; set; }
    public bool selected { get; set; }
    public Rank rank { get; set; } = Rank.Star1;
}

public enum Rank
{
    Star1,
    Star2,
    Star3,
    Star4,
    Star5,
    Illumination1,
    Illumination2,
    Illumination3,
    Awakened
}
