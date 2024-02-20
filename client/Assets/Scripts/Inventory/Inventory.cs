using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField]
    GameObject itemPrefab;
    [SerializeField]
    GameObject itemsContainer;

    void Start()
    {
        foreach(Item item in GlobalUserData.Instance.User.items) {
            Instantiate(itemPrefab, itemsContainer.transform);
        }
    }
}
