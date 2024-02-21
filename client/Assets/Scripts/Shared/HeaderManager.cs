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
        // Hardcoded userId, should be changed to the userId you have in your database. Better yet should be to build the full user login and creation with the new websocket implemetatiom
        SocketConnection.GetUser("76eff72a-02be-4485-92a0-11009fff2ca2");
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
