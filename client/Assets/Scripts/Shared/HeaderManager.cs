using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HeaderManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI username;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI xp;
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI gems;
	[SerializeField] TextMeshProUGUI scrolls;

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
    }

    void UpdateLevelValues()
    {
        level.text = "Lv. " + user.User.level.ToString();
        xp.text = user.User.experience.ToString() + "/" + user.User.experienceToNextLevel.ToString();
    }
}
