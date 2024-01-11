using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AcceptBehaviour : MonoBehaviour
{
    [SerializeField]
    public List<Box> boxes;

    [SerializeField]
    private TextMeshProUGUI item_name;

    public Box box;

    public void SetBoxByName(string box_name){
        box = boxes.Find(box => box_name == box.name);
        item_name.text = box_name;
    }

    public void PullBox(){
        GachaManager.RollCharacter(box);
    }
}
