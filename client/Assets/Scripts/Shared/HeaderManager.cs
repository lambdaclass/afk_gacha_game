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

    static User user;

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
            user = GlobalUserData.Instance.User;
        }

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
