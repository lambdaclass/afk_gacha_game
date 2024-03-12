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
    Experience
}

// public static class CurrencyParser
// {
//     public static Currency FromString(string currencyString)
//     {
//         switch (currencyString.ToLower().Replace(" ", ""))
//         {
//             case "gold":
//                 return Currency.Gold;
//             case "gems":
//                 return Currency.Gems;
//             case "summonscrolls":
//                 return Currency.SummonScrolls;
//             case "heroicscrolls":
//                 return Currency.HeroicScrolls;
//             case "experience":
//                 return Currency.Experience;
//             default:
//                 throw new ArgumentException("Invalid currency string", nameof(currencyString));
//         }
//     }
// }
