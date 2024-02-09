using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemList : MonoBehaviour
{
    void Start()
    {
        Debug.Log("wqdq");
        foreach(Item item in GlobalUserData.Instance.User.items) {
            Debug.Log($"{item.template.name}, {item.level}");
        }
    }
}
