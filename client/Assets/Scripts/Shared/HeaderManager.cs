using System.Collections;
using System.Collections.Generic;
using DuloGames.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class HeaderManager : MonoBehaviour
{
	[SerializeField]
	TextMeshProUGUI username;
    
	[SerializeField]
	TextMeshProUGUI level;
    
	[SerializeField]
	UIProgressBar progressBarXp;
    
	[SerializeField]
	TextMeshProUGUI gold;
    
	[SerializeField]
	TextMeshProUGUI gems;
	
	[SerializeField]
	TextMeshProUGUI scrolls;

	[SerializeField]
	TextMeshProUGUI fertalizer;

    static GlobalUserData user;

    // static bool infoHasBeenSet = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetUserAndContinue());
    }

    private IEnumerator GetUserAndContinue()
    {
        if(user == null) {
            yield return new WaitUntil(() => GlobalUserData.Instance.User != null);
            user = GlobalUserData.Instance;
        }

        username.text = user.User.username;

        user.OnCurrencyModified.AddListener(UpdateCurrencyValues);
        user.OnLevelModified.AddListener(UpdateLevelValues);

        UpdateCurrencyValues();
        UpdateLevelValues();
    }

    void UpdateCurrencyValues()
    {
        gold.text = user.GetCurrency(Currency.Gold).ToString();
        gems.text = user.GetCurrency(Currency.Gems).ToString();
		scrolls.text = user.GetCurrency(Currency.SummonScrolls).ToString();
		fertalizer.text = user.GetCurrency(Currency.Fertilizer).ToString();
    }

    void UpdateLevelValues()
    {
        level.text = "Level " + user.User.level.ToString();
		progressBarXp.fillAmount = user.User.experience / (float)user.User.experienceToNextLevel;
    }
}
