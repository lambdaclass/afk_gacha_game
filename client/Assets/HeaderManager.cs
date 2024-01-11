using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HeaderManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI username;
    [SerializeField] TextMeshProUGUI gold;
    [SerializeField] TextMeshProUGUI gems;

    User user;

    // Start is called before the first frame update
    void Start()
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;

        user = globalUserData.User;

        username.text = user.username;

        UpdateCurrencyValues();
    }

    void UpdateCurrencyValues()
    {
        gold.text = user.currencies["gold"].ToString();
        gems.text = user.currencies["gems"].ToString();
    }
}
