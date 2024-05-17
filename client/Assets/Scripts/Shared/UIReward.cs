using UnityEngine;

public abstract class UIReward
{
    public Sprite Sprite()
    {
        return Resources.Load<Sprite>("UI/Rewards/" + RewardType());
    }

    public abstract int Amount();

    public abstract string RewardType();
}

public class CurrencyUIReward : UIReward
{
    private CurrencyValue value;

    public CurrencyUIReward(Currency currencyName, int currencyAmount)
    {
        CurrencyValue currencyValue = new CurrencyValue();
        currencyValue.currency = currencyName;
        currencyValue.value = currencyAmount;

        value = currencyValue;
    }

    public override int Amount() { return value.value; }

    public override string RewardType() { return value.currency.ToString(); }
}

public class ExperienceUIReward : UIReward
{
    private int value;

    public ExperienceUIReward(int amount)
    {
        value = amount;
    }

    public override int Amount() { return value; }
    public override string RewardType() { return "experience"; }
}

//// When we implement item rewards:
// public class ItemUIReward : UIReward {
//     public Item item;
//     public int amount;
// }
