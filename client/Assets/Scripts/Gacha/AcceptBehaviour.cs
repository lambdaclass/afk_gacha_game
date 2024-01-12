using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AcceptBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI item_name;

    private Box box;

    public void SetBox(Box newBox){
        box = newBox;
        item_name.text = box.name;
    }

    public void Accept(){
        PullBox();
        SubstractMoney();
    }
    private void PullBox(){
        GachaManager.RollCharacter(box);
    }

    private void SubstractMoney() {
        User user = GlobalUserData.Instance.User;

        foreach (var cost in box.GetCost()) {
            string currency = cost.Key;
            int costAmount = cost.Value;

            int playerMoney = (int) user.GetCurrency(currency);
            
            user.ModifyCurrency(currency, -costAmount);
        }
    }
}
