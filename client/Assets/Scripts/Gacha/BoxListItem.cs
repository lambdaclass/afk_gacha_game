using System.Collections.Generic;
using UnityEngine;

public class BoxListItem : MonoBehaviour
{
    [SerializeField] Box box;
    [SerializeField] GameObject confirmPopUp;

    public void SelectBox()
    {
        GlobalUserData globalUserData = GlobalUserData.Instance;

        if (CanUserBuyItem(globalUserData.User, box.GetCost())) {
            confirmPopUp.GetComponent<AcceptBehaviour>().SetBox(box);
            confirmPopUp.SetActive(true);
        }
    }

    static bool CanUserBuyItem(User user, Dictionary<Currency, int> itemCosts)
    {
        foreach (var cost in itemCosts)
        {
            Currency currency = cost.Key;
            int costAmount = cost.Value;

            int? playerMoney = user.GetCurrency(currency);
            
            // Check if the player has enough of each currency
            if (playerMoney == null) {
                
                //// Do frontend stuff

                return false;
            }

            if (playerMoney < costAmount)
            {
                // Player doesn't have enough of this currency

                //// Do Frontend stuff

                return false;
            }
        }

        // Player has enough of all currencies
        return true;
    }
}
