using System;

[System.Serializable]
public struct CurrencyValue {
    public Currency currency;
    public int value;
}

public enum Currency {
    Gold,
    Gems,
    SummonScrolls,
    HeroicScrolls,
    Experience,
	ArcaneCrystals,
	HeroSouls,
	MysticSummonScrolls
}
