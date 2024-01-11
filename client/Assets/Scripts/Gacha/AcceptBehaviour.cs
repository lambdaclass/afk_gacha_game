using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AcceptBehaviour : MonoBehaviour
{
    [SerializeField]
    public List<Box> boxes;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private GachaManager gachaManager;

    public Box box;

    public void SetBoxByName(string boxName){
        print("SetBoxByName");
        box = boxes.Find(box => boxName == box.name);
        //itemName.text = boxName;
    }

    public void PullBox(){
        print("PullBox");
        gachaManager.RollCharacter(box);
    }
}
