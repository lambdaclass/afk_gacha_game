using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class HeaderManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI username;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI xp;
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI gems;

    User user;

    // Start is called before the first frame update
    async void Start()
    {
        await SocketConnection.Connect();
        SocketConnection.GetUser("b52949a9-eb28-49d4-a614-d243452ca6e7");
        StartCoroutine(GetUserAndContinue());
    }

    private IEnumerator GetUserAndContinue()
    {
        yield return new WaitUntil(() => GlobalUserData.Instance.User != null);

        user = GlobalUserData.Instance.User;

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
