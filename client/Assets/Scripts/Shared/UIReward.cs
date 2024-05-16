using UnityEngine;
using System.Linq;

public abstract class UIReward
{
	public Sprite Sprite()
	{
		return GlobalUserData.Instance.AvailableCurrencies.Single(currency => currency.name == RewardType()).image;
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
	public override string RewardType() { return "Experience"; }
}

//// When we implement item rewards:
// public class ItemUIReward : UIReward {
//     public Item item;
//     public int amount;
// }
