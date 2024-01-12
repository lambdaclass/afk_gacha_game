using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AcceptBehaviour : MonoBehaviour
{
    public List<Box> boxes;

    [SerializeField]
    private TextMeshProUGUI itemName;

    [SerializeField]
    private GachaManager gachaManager;

    public Box box;

    public void SetBoxByName(string boxName){
        box = boxes.Find(box => boxName == box.name);
        print(boxName);
        itemName.text = boxName;
    }

    public void PullBox(){
        gachaManager.RollCharacter(box);
    }
}
