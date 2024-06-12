using UnityEngine;
using System.Linq;

public abstract class UIReward
{
    public Sprite Sprite()
    {
        return RewardType() switch
        {
            "experience" => GlobalUserData.Instance.AvailableCurrencies.Single(currency => currency.name == "Experience").image,
            "item" => ((ItemUIReward)this).itemReward.itemTemplate.icon,
            "unit" => ((UnitUIReward)this).unitReward.character.defaultSprite,
            "currency" => ((CurrencyUIReward)this).currencyReward.currency.image,
            _ => throw new System.Exception("Reward type " + RewardType() + " not recognized")
        };
    }

    public abstract int Amount();
    public abstract string RewardType();
    public abstract string GetName();
}

public class CurrencyUIReward : UIReward
{
    public CurrencyReward currencyReward;

    public CurrencyUIReward(CurrencyReward currencyReward)
    {
        this.currencyReward = currencyReward;
    }

    public override int Amount() { return currencyReward.amount; }
    public override string RewardType() { return "currency"; }
    public override string GetName() { return currencyReward.currency.name; }
}

public class ExperienceUIReward : UIReward
{
    private readonly int value;

    public ExperienceUIReward(int amount)
    {
        value = amount;
    }

    public override int Amount() { return value; }
    public override string RewardType() { return "experience"; }
    public override string GetName() { return "Experience"; }
}

public class ItemUIReward : UIReward
{
    public ItemReward itemReward;

    public ItemUIReward(ItemReward itemReward)
    {
        this.itemReward = itemReward;
    }

    public override int Amount() { return itemReward.amount; }
    public override string RewardType() { return "item"; }
    public override string GetName() { return itemReward.itemTemplate.name; }
}

public class UnitUIReward : UIReward
{
    public UnitReward unitReward;
    public UnitUIReward(UnitReward unitReward)
    {
        this.unitReward = unitReward;
    }

    public override int Amount() { return unitReward.amount; }
    public override string RewardType() { return "unit"; }
    public override string GetName() { return unitReward.character.name; }
}
