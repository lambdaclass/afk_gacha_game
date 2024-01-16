using System;

[System.Serializable]
public struct CurrencyValue {
    public Currency name;
    public int value;
}

public enum Currency {
    Gold,
    Gems,
    Scrolls,
    HeroicScrolls
}
