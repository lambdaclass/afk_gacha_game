using System;
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
        GlobalUserData.Instance.User.SubstractCurrency(box.GetCost());
    }

    private void PullBox(){
        GachaManager.RollCharacter(box);

    }
}
