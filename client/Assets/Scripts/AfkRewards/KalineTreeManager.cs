using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class KalineTreeManager : MonoBehaviour
{
	[SerializeField] TMP_Text goldLevelUpCost;
	[SerializeField] TMP_Text fertilizerLevelUpCost;
	[SerializeField] TMP_Text goldAfkRewardRate;
	[SerializeField] TMP_Text heroSoulsAfkRewardRate;
	[SerializeField] TMP_Text experienceAfkRewardRate;
	[SerializeField] TMP_Text arcaneCrystalsAfkRewardRate;
	[SerializeField] TMP_Text kalineTreeLevel;
	[SerializeField] GameObject confirmPopUp;
	[SerializeField] GameObject insufficientCurrencyPopup;
	[SerializeField] GameObject afkRewardDetailUI;
	[SerializeField] GameObject afkRewardsContainer;

	private const string EMPTY_AFK_REWARD = "0 (0/m)";
	private const int SECONDS_IN_DAY = 86400;

	void Start()
	{
		GlobalUserData user = GlobalUserData.Instance;
		goldLevelUpCost.text = user.User.kalineTreeLevel.goldLevelUpCost.ToString();
		fertilizerLevelUpCost.text = user.User.kalineTreeLevel.goldLevelUpCost.ToString();
		kalineTreeLevel.text = $"Level {user.User.kalineTreeLevel.level}";
		SetAfkRewardRatesTexts(user.User);
	}
	public void ShowRewards()
	{
		GlobalUserData user = GlobalUserData.Instance;
		SocketConnection.Instance.GetAfkRewards(user.User.id, (afkRewards) =>
		{
			foreach (Transform child in afkRewardsContainer.transform)
			{
				Destroy(child.gameObject);
			}

			foreach (var afkReward in afkRewards)
			{
				GameObject afkRewardGO = Instantiate(afkRewardDetailUI, afkRewardsContainer.transform);
				AfkRewardDetail afkRewardDetail = afkRewardGO.GetComponent<AfkRewardDetail>();
				afkRewardDetail.SetData(GlobalUserData.Instance.AvailableCurrencies.Single(currency => currency.name == afkReward.currency).image, $"{afkReward.amount} ({user.User.kalineTreeLevel.afkRewardRates.Single(arr => arr.currency == afkReward.currency).rate * 60}/m)");
			}

			confirmPopUp.SetActive(true);
		});
	}

	public void ClaimRewards()
	{
		SocketConnection.Instance.ClaimAfkRewards(GlobalUserData.Instance.User.id, (userReceived) =>
		{
			GlobalUserData userToUpdate = GlobalUserData.Instance;
			Dictionary<string, int> currenciesToAdd = new Dictionary<string, int>();

			userReceived.currencies.Select(c => c.Key).ToList().ForEach(c =>
			{
				if (!currenciesToAdd.ContainsKey(c))
				{
					currenciesToAdd.Add(c, userReceived.currencies[c] - userToUpdate.GetCurrency(c).Value);
				}
			});
			currenciesToAdd.Add("Experience", userReceived.experience - userToUpdate.User.experience);
			userToUpdate.AddCurrencies(currenciesToAdd);
		});
		confirmPopUp.SetActive(false);
	}

	public void LevelUpKalineTree()
	{
		SocketConnection.Instance.LevelUpKalineTree(
			GlobalUserData.Instance.User.id,
			(userReceived) =>
			{
				GlobalUserData userToUpdate = GlobalUserData.Instance;
				Dictionary<string, int> currenciesToAdd = new Dictionary<string, int>();

				userReceived.currencies.Select(c => c.Key).ToList().ForEach(c =>
				{
					if (!currenciesToAdd.ContainsKey(c))
					{
						userToUpdate.SetCurrencyAmount(c, userReceived.currencies[c]);
					}
				});
				userReceived.kalineTreeLevel.afkRewardRates.ForEach(afkRewardRate =>
				{
					userToUpdate.User.kalineTreeLevel.afkRewardRates.Add(afkRewardRate);
				});
				UpdateRatesAndLevelUpCosts(userReceived);
				kalineTreeLevel.text = $"Level {userReceived.kalineTreeLevel.level}";
			},
			(reason) =>
			{
				if (reason == "cant_afford")
				{
					insufficientCurrencyPopup.SetActive(true);
				}
			}
		);
	}

	public void UpdateRatesAndLevelUpCosts(User user)
	{
		goldLevelUpCost.text = user.kalineTreeLevel.goldLevelUpCost.ToString();
		fertilizerLevelUpCost.text = user.kalineTreeLevel.goldLevelUpCost.ToString();
		SetAfkRewardRatesTexts(user);
	}

	private void SetAfkRewardRatesTexts(User user)
	{
		foreach (AfkRewardRate afkRewardRate in user.kalineTreeLevel.afkRewardRates)
		{
			switch (afkRewardRate.currency)
			{
				case "Gold":
					goldAfkRewardRate.text = GetAfkRewardRateText(afkRewardRate.rate);
					break;
				case "Hero Souls":
					heroSoulsAfkRewardRate.text = GetAfkRewardRateText(afkRewardRate.rate);
					break;
				case "Experience":
					experienceAfkRewardRate.text = GetAfkRewardRateText(afkRewardRate.rate);
					break;
				case "Arcane Crystals":
					arcaneCrystalsAfkRewardRate.text = GetAfkRewardRateText(afkRewardRate.rate);
					break;
			}
		}
	}

	private string GetAfkRewardRateText(float rate)
	{
		return $"{rate * SECONDS_IN_DAY}/day";
	}
}
