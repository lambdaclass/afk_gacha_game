using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AcceptBehaviour : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI itemName;

    private Box box;

    [SerializeField]
    private GachaManager gachaManager;

    public void SetBox(Box newBox){
        box = newBox;
        itemName.text = box.name;
    }

    public void Accept(){
        PullBox();
        GlobalUserData.Instance.User.SubstractCurrency(box.GetCost());
    }

    private void PullBox(){
        gachaManager.RollCharacter(box);
    }
}
