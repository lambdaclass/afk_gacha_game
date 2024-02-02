using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeaderManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI username;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI xp;
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI gems;

    User user;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetUserAndContinue());

        // GlobalUserData globalUserData = GlobalUserData.Instance;
        // user = globalUserData.User;

        // username.text = user.username;

        // user.OnCurrencyModified.AddListener(UpdateCurrencyValues);
        // user.OnLevelModified.AddListener(UpdateLevelValues);

        // UpdateCurrencyValues();
        // UpdateLevelValues();
    }

    private IEnumerator GetUserAndContinue()
    {
        yield return new WaitUntil(() => GlobalUserData.Instance.User != null);

        GlobalUserData globalUserData = GlobalUserData.Instance;
        user = globalUserData.User;

        username.text = user.username;

        user.OnCurrencyModified.AddListener(UpdateCurrencyValues);
        user.OnLevelModified.AddListener(UpdateLevelValues);

        UpdateCurrencyValues();
        UpdateLevelValues();
    }

    void UpdateCurrencyValues()
    {
        gold.text = user.GetCurrency(Currency.Gold).ToString();
        gems.text = user.GetCurrency(Currency.Gems).ToString();
    }

    void UpdateLevelValues()
    {
        level.text = "Lv. " + user.level.ToString();
        xp.text = user.experience.ToString() + "/" + user.experienceToNextLevel.ToString();
    }
}
