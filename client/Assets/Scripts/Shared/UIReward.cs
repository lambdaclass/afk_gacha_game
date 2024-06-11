using UnityEngine;
using System.Linq;

public abstract class UIReward
{
    public Sprite Sprite()
    {
        return RewardType() switch
        {
            "experience" => GlobalUserData.Instance.AvailableCurrencies.Single(currency => currency.name == "experience").image,
            "item" => ((ItemUIReward)this).itemReward.itemTemplate.icon,
            "unit" => ((UnitUIReward)this).unitReward.character.defaultSprite,
            // If it's not any of the other 3, then it's a Currency
            _ => GlobalUserData.Instance.AvailableCurrencies.Single(currency => currency.name == RewardType()).image
        };
    }

    public abstract int Amount();

    public abstract string RewardType();
}

public class CurrencyUIReward : UIReward
{
    private string currency;
    private int amount;

    public CurrencyUIReward(string currency, int amount)
    {
        this.currency = currency;
        this.amount = amount;
    }

    public override int Amount() { return amount; }

    public override string RewardType() { return currency; }
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

public class ItemUIReward : UIReward
{
    public ItemReward itemReward;

    public ItemUIReward(ItemReward itemReward)
    {
        this.itemReward = itemReward;
    }

    public override int Amount() { return 1; }

    public override string RewardType() { return "item"; }
}

public class UnitUIReward : UIReward
{
    public UnitReward unitReward;
    public UnitUIReward(UnitReward unitReward)
    {
        this.unitReward = unitReward;
    }

    public override int Amount() { return 1; }

    public override string RewardType() { return "unit"; }
}
