using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxListItem : MonoBehaviour
{
    [SerializeField] Box box;
    [SerializeField] GameObject confirmPopUp;

    public void SelectBox()
    {
        confirmPopUp.GetComponent<AcceptBehaviour>().SetBox(box);
        confirmPopUp.SetActive(true);
    }
}
