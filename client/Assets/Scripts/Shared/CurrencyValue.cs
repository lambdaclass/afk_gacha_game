[System.Serializable]
public struct CurrencyValue
{
    public Currency currency;
    public int value;
}

public enum Currency
{
    Experience,
    Gold,
    Gems,
    ArcaneCrystals,
    HeroSouls,
    SummonScrolls,
    HeroicScrolls,
    MysticSummonScrolls,
    Fertilizer
}